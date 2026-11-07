using Newtonsoft.Json;

namespace openjob_sdk_csharp_agent.common.request.agent
{
    /// <summary>
    /// 任务实例检查请求体
    /// </summary>
    public class AgentCheckRequest
    {
        /// <summary>
        /// 任务实例ID
        /// </summary>
        [JsonProperty("jobInstanceId")]
        public long? JobInstanceId { get; set; }
    }
}
