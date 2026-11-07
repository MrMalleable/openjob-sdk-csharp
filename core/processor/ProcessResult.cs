namespace openjob_sdk_csharp_agent.core.processor
{
    /// <summary>
    /// 处理器执行结果
    /// </summary>
    public class ProcessResult
    {
        public bool Success { get; init; }
        public string Result { get; init; } = string.Empty;

        public static ProcessResult Ok(string result = "success") => new()
        {
            Success = true,
            Result = result
        };

        public static ProcessResult Fail(string result) => new()
        {
            Success = false,
            Result = result
        };
    }
}
