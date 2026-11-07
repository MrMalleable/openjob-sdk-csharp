using Newtonsoft.Json;

namespace openjob_sdk_csharp_agent.common.request.cluster
{
    /// <summary>
    /// 上报实例状态请求体
    /// </summary>
    public class ClusterHandleStatusRequest
    {
        [JsonProperty("jobId")]
        public long? JobId { get; set; }

        [JsonProperty("jobInstanceId")]
        public long? JobInstanceId { get; set; }

        /// <summary>
        /// 5 执行中 / 10 成功 / 15 失败 / 20 停止 / 25 取消
        /// </summary>
        [JsonProperty("status")]
        public long? Status { get; set; }

        /// <summary>
        /// 0 默认 / 1 执行失败 / 5 执行超时
        /// </summary>
        [JsonProperty("failStatus")]
        public long? FailStatus { get; set; }

        [JsonProperty("result")]
        public string? Result { get; set; }
    }
}
