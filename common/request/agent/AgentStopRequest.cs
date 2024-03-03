using Newtonsoft.Json;

namespace openjob_sdk_csharp_agent.common.request.agent
{
    /// <summary>
    /// 终止任务实例请求体
    /// </summary>
    public class AgentStopRequest
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
    }
}
