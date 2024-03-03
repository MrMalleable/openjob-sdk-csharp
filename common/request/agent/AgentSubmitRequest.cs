using Newtonsoft.Json;

namespace openjob_sdk_csharp_agent.common.request.agent
{
    /// <summary>
    /// 执行任务实例的请求体
    /// </summary>
    public class AgentSubmitRequest
    {
        /// <summary>
        /// 任务ID
        /// </summary>
        [JsonProperty("jobId")]
        public long? JobId { get; set; }
        /// <summary>
        /// 任务实例ID
        /// </summary>
        [JsonProperty("jobInstanceId")]
        public long? JobInstanceId { get; set; }
        /// <summary>
        /// 任务调度版本
        /// </summary>
        [JsonProperty("dispatchVersion")]
        public long? DispatchVersion { get; set; }
        /// <summary>
        /// 参数类型
        /// </summary>
        [JsonProperty("jobParamType")]
        public string? JobParamType { get; set; }
        /// <summary>
        /// 任务参数
        /// </summary>
        [JsonProperty("jobParams")]
        public string? JobParams { get; set; }
        /// <summary>
        /// 任务扩展参数类型
        /// </summary>
        [JsonProperty("jobExtendParamsType")]
        public string? JobExtendParamsType { get; set; }
        /// <summary>
        /// 任务扩展参数
        /// </summary>
        [JsonProperty("jobExtendParams")]
        public string? JobExtendParams { get; set; }
        /// <summary>
        /// 执行器类型
        /// </summary>
        [JsonProperty("processorType")]
        public string? ProcessorType { get; set; }
        /// <summary>
        /// 执行器名称
        /// </summary>
        [JsonProperty("processorInfo")]
        public string? ProcessorInfo { get; set; }
        /// <summary>
        /// 执行类型
        /// </summary>
        [JsonProperty("executeType")]
        public string? ExecuteType { get; set; }
        /// <summary>
        /// 失败重试次数
        /// </summary>
        [JsonProperty("failRetryTimes")]
        public int? FailRetryTimes { get; set; }
        /// <summary>
        /// 失败重试间隔(s)
        /// </summary>
        [JsonProperty("failRetryInterval")]
        public int? FailRetryInterval { get; set; }
        /// <summary>
        /// 执行超时时间(s)，0=无超时
        /// </summary>
        [JsonProperty("executeTimeout")]
        public int? ExecuteTimeout { get; set; }
        /// <summary>
        /// 并发(同一个任务最大执行并发)
        /// </summary>
        [JsonProperty("concurrency")]
        public int? Concurrency { get; set; }
        /// <summary>
        /// 时间表达式类型
        /// </summary>
        [JsonProperty("timeExpressionType")]
        public string? TimeExpressionType { get; set; }
        /// <summary>
        /// 时间表达式
        /// </summary>
        [JsonProperty("timeExpression")]
        public string? TimeExpression { get; set; }
        /// <summary>
        /// 是否一次性任务 1=是 2=否
        /// </summary>
        [JsonProperty("executeOnce")]
        public int? ExecuteOnce { get; set; }
    }
}
