namespace openjob_sdk_csharp_agent.core.processor
{
    /// <summary>
    /// 任务处理器 SPI
    /// </summary>
    public interface IJobProcessor
    {
        /// <summary>
        /// 处理器名称，对应 OpenJob 控制台配置的 processorInfo
        /// </summary>
        string Name { get; }

        Task<ProcessResult> ProcessAsync(JobContext context);
    }
}
