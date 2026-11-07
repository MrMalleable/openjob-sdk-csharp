using Microsoft.Extensions.Options;
using openjob_sdk_csharp_agent.config;

namespace openjob_sdk_csharp_agent.core
{
    /// <summary>
    /// Worker 生命周期：注册、心跳、优雅下线
    /// </summary>
    public class HeartBeatService : BackgroundService
    {
        private readonly ILogger<HeartBeatService> _logger;
        private readonly JobInstanceService _jobInstanceService;
        private readonly OpenJobConfig _config;

        public HeartBeatService(
            ILogger<HeartBeatService> logger,
            JobInstanceService jobInstanceService,
            IOptions<OpenJobConfig> options)
        {
            _logger = logger;
            _jobInstanceService = jobInstanceService;
            _config = options.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var registered = false;
            while (!stoppingToken.IsCancellationRequested && !registered)
            {
                try
                {
                    registered = await _jobInstanceService.RegisterOnAsync(stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                if (!registered)
                {
                    _logger.LogWarning("Worker register retry in 3 seconds");
                    try
                    {
                        await Task.Delay(TimeSpan.FromSeconds(3), stoppingToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                        return;
                    }
                }
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await _jobInstanceService.SendHeartBeatAsync(stoppingToken).ConfigureAwait(false);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Heartbeat loop error");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_config.HeartBeatInterval), stoppingToken)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("HeartBeatService stopping, unregister worker...");
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            await _jobInstanceService.RegisterOffAsync(cts.Token).ConfigureAwait(false);
            await base.StopAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}
