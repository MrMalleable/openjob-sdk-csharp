using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using openjob_sdk_csharp_agent.common.constant;
using openjob_sdk_csharp_agent.common.helper;
using openjob_sdk_csharp_agent.common.request.agent;
using openjob_sdk_csharp_agent.common.request.cluster;
using openjob_sdk_csharp_agent.config;
using openjob_sdk_csharp_agent.core.processor;

namespace openjob_sdk_csharp_agent.core
{
    /// <summary>
    /// 任务实例核心服务：注册、心跳、执行、停止、状态上报
    /// </summary>
    public class JobInstanceService
    {
        private readonly ILogger<JobInstanceService> _logger;
        private readonly OpenJobConfig _openJobConfig;
        private readonly IClusterApiClient _clusterApiClient;
        private readonly JobProcessorRegistry _processorRegistry;
        private readonly ConcurrentDictionary<long, RunningJobInstance> _runningInstances = new();

        public JobInstanceService(
            ILogger<JobInstanceService> logger,
            IOptions<OpenJobConfig> options,
            IClusterApiClient clusterApiClient,
            JobProcessorRegistry processorRegistry)
        {
            _logger = logger;
            _openJobConfig = options.Value;
            _clusterApiClient = clusterApiClient;
            _processorRegistry = processorRegistry;
            _logger.LogInformation(
                "JobInstanceService initialized. address={Address}, processors=[{Processors}]",
                _openJobConfig.Address,
                string.Join(",", _processorRegistry.GetRegisteredNames()));
        }

        public OpenJobConfig GetConfiguration() => _openJobConfig;

        public bool IsRegistered => _openJobConfig.AppId.HasValue;

        /// <summary>
        /// Worker 上线
        /// </summary>
        public async Task<bool> RegisterOnAsync(CancellationToken cancellationToken = default)
        {
            var response = await _clusterApiClient.RegisterOnAsync(cancellationToken).ConfigureAwait(false);
            if (response?.Status != 200 || response.Data?.AppId == null)
            {
                _logger.LogError(
                    "Worker register failed. status={Status}, message={Message}",
                    response?.Status, response?.Message);
                return false;
            }

            _openJobConfig.AppId = response.Data.AppId;
            _logger.LogInformation(
                "Worker register success. appId={AppId}, address={Address}",
                _openJobConfig.AppId, _openJobConfig.Address);
            return true;
        }

        /// <summary>
        /// Worker 下线
        /// </summary>
        public async Task RegisterOffAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _clusterApiClient.RegisterOffAsync(cancellationToken).ConfigureAwait(false);
                _logger.LogInformation("Worker unregister success. address={Address}", _openJobConfig.Address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Worker unregister failed. address={Address}", _openJobConfig.Address);
            }
        }

        /// <summary>
        /// 发送心跳
        /// </summary>
        public async Task SendHeartBeatAsync(CancellationToken cancellationToken = default)
        {
            if (!_openJobConfig.AppId.HasValue)
            {
                _logger.LogWarning("Skip heartbeat because AppId is empty");
                return;
            }

            var response = await _clusterApiClient
                .HeartBeatAsync(GetJobInstanceIds(), cancellationToken)
                .ConfigureAwait(false);

            if (response?.Status != 200)
            {
                _logger.LogWarning(
                    "Heartbeat failed. status={Status}, message={Message}",
                    response?.Status, response?.Message);
            }
        }

        /// <summary>
        /// 当前正在执行的任务实例 ID
        /// </summary>
        public IList<long> GetJobInstanceIds()
        {
            return _runningInstances.Keys.ToList();
        }

        /// <summary>
        /// 接收并异步执行任务
        /// </summary>
        public async Task<(bool accepted, string message)> SubmitAsync(AgentSubmitRequest request)
        {
            if (request.JobInstanceId is null or <= 0)
            {
                return (false, "jobInstanceId is required");
            }

            if (string.IsNullOrWhiteSpace(request.ProcessorInfo))
            {
                return (false, "processorInfo is required");
            }

            if (!_processorRegistry.TryGet(request.ProcessorInfo, out var processor) || processor == null)
            {
                return (false, $"processor not found: {request.ProcessorInfo}");
            }

            var cts = new CancellationTokenSource();
            var running = new RunningJobInstance(request, cts, processor);

            if (!_runningInstances.TryAdd(request.JobInstanceId.Value, running))
            {
                cts.Dispose();
                return (false, $"job instance already running: {request.JobInstanceId}");
            }

            _ = Task.Run(() => ExecuteJobAsync(running), CancellationToken.None);
            await ReportStatusAsync(
                request.JobId,
                request.JobInstanceId,
                InstanceStatusConstant.Running,
                FailStatusConstant.None,
                "running").ConfigureAwait(false);

            return (true, "accepted");
        }

        /// <summary>
        /// 终止任务实例
        /// </summary>
        public bool Stop(AgentStopRequest request)
        {
            if (request.JobInstanceId is null or <= 0)
            {
                return false;
            }

            if (!_runningInstances.TryGetValue(request.JobInstanceId.Value, out var running))
            {
                return false;
            }

            running.CancellationTokenSource.Cancel();
            return true;
        }

        /// <summary>
        /// 检查任务实例是否仍在运行
        /// </summary>
        public bool Check(long? jobInstanceId)
        {
            return jobInstanceId.HasValue && _runningInstances.ContainsKey(jobInstanceId.Value);
        }

        private async Task ExecuteJobAsync(RunningJobInstance running)
        {
            var request = running.Request;
            var jobInstanceId = request.JobInstanceId!.Value;

            try
            {
                using var timeoutCts = CreateTimeoutCts(request.ExecuteTimeout, running.CancellationTokenSource.Token);
                var context = JobContext.FromRequest(request, timeoutCts.Token);
                var result = await running.Processor.ProcessAsync(context).ConfigureAwait(false);

                if (running.CancellationTokenSource.IsCancellationRequested)
                {
                    await ReportStatusAsync(
                        request.JobId,
                        request.JobInstanceId,
                        InstanceStatusConstant.Stop,
                        FailStatusConstant.None,
                        "stopped").ConfigureAwait(false);
                    return;
                }

                if (result.Success)
                {
                    await ReportStatusAsync(
                        request.JobId,
                        request.JobInstanceId,
                        InstanceStatusConstant.Success,
                        FailStatusConstant.None,
                        result.Result).ConfigureAwait(false);
                }
                else
                {
                    await ReportStatusAsync(
                        request.JobId,
                        request.JobInstanceId,
                        InstanceStatusConstant.Fail,
                        FailStatusConstant.ExecuteFail,
                        result.Result).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                var timeout = request.ExecuteTimeout is > 0
                              && !running.CancellationTokenSource.IsCancellationRequested;
                await ReportStatusAsync(
                    request.JobId,
                    request.JobInstanceId,
                    timeout ? InstanceStatusConstant.Fail : InstanceStatusConstant.Stop,
                    timeout ? FailStatusConstant.ExecuteTimeout : FailStatusConstant.None,
                    timeout ? "execute timeout" : "stopped").ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Job execution failed. jobId={JobId}, jobInstanceId={JobInstanceId}",
                    request.JobId, jobInstanceId);
                await ReportStatusAsync(
                    request.JobId,
                    request.JobInstanceId,
                    InstanceStatusConstant.Fail,
                    FailStatusConstant.ExecuteFail,
                    ex.Message).ConfigureAwait(false);
            }
            finally
            {
                if (_runningInstances.TryRemove(jobInstanceId, out var removed))
                {
                    removed.CancellationTokenSource.Dispose();
                }
            }
        }

        private async Task ReportStatusAsync(
            long? jobId,
            long? jobInstanceId,
            int status,
            int failStatus,
            string result)
        {
            var request = new ClusterHandleStatusRequest
            {
                JobId = jobId,
                JobInstanceId = jobInstanceId,
                Status = status,
                FailStatus = failStatus,
                Result = result
            };

            var response = await _clusterApiClient.HandleStatusAsync(request).ConfigureAwait(false);
            if (response?.Status != 200)
            {
                _logger.LogWarning(
                    "Report status failed. jobInstanceId={JobInstanceId}, status={Status}, message={Message}",
                    jobInstanceId, response?.Status, response?.Message);
            }
        }

        private static CancellationTokenSource CreateTimeoutCts(int? executeTimeoutSeconds, CancellationToken parent)
        {
            if (executeTimeoutSeconds is null or <= 0)
            {
                return CancellationTokenSource.CreateLinkedTokenSource(parent);
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(parent);
            cts.CancelAfter(TimeSpan.FromSeconds(executeTimeoutSeconds.Value));
            return cts;
        }

        private sealed class RunningJobInstance
        {
            public RunningJobInstance(
                AgentSubmitRequest request,
                CancellationTokenSource cancellationTokenSource,
                IJobProcessor processor)
            {
                Request = request;
                CancellationTokenSource = cancellationTokenSource;
                Processor = processor;
            }

            public AgentSubmitRequest Request { get; }
            public CancellationTokenSource CancellationTokenSource { get; }
            public IJobProcessor Processor { get; }
        }
    }
}
