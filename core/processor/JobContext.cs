using openjob_sdk_csharp_agent.common.request.agent;

namespace openjob_sdk_csharp_agent.core.processor
{
    /// <summary>
    /// 任务执行上下文
    /// </summary>
    public class JobContext
    {
        public long? JobId { get; init; }
        public long? JobInstanceId { get; init; }
        public long? DispatchVersion { get; init; }
        public string? JobParamType { get; init; }
        public string? JobParams { get; init; }
        public string? JobExtendParamsType { get; init; }
        public string? JobExtendParams { get; init; }
        public string? ProcessorType { get; init; }
        public string? ProcessorInfo { get; init; }
        public string? ExecuteType { get; init; }
        public CancellationToken CancellationToken { get; init; }

        public static JobContext FromRequest(AgentSubmitRequest request, CancellationToken cancellationToken)
        {
            return new JobContext
            {
                JobId = request.JobId,
                JobInstanceId = request.JobInstanceId,
                DispatchVersion = request.DispatchVersion,
                JobParamType = request.JobParamType,
                JobParams = request.JobParams,
                JobExtendParamsType = request.JobExtendParamsType,
                JobExtendParams = request.JobExtendParams,
                ProcessorType = request.ProcessorType,
                ProcessorInfo = request.ProcessorInfo,
                ExecuteType = request.ExecuteType,
                CancellationToken = cancellationToken
            };
        }
    }
}
