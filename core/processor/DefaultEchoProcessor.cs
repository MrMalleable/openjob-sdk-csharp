namespace openjob_sdk_csharp_agent.core.processor
{
    /// <summary>
    /// 默认示例处理器，便于联调验证
    /// </summary>
    public class DefaultEchoProcessor : IJobProcessor
    {
        public string Name => "defaultEchoProcessor";

        public Task<ProcessResult> ProcessAsync(JobContext context)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            var result = $"echo jobId={context.JobId}, instanceId={context.JobInstanceId}, params={context.JobParams}";
            return Task.FromResult(ProcessResult.Ok(result));
        }
    }
}
