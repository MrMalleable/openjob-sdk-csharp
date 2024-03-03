using Newtonsoft.Json;

namespace openjob_sdk_csharp_agent.common.request.cluster
{
    /// <summary>
    /// 上报实例状态请求体
    /// </summary>
    public class ClusterHandleStatusRequest
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
        /// 任务状态
        /// 5: 执行中
        /// 10：执行成功
        /// 15：执行失败
        /// 20：任务停止
        /// 25：任务取消
        /// </summary>
        [JsonProperty("status")]
        public long? Status { get; set; }
        /// <summary>
        /// 失败状态
        /// 0：默认状态(成功)
        /// 1：执行失败
        /// 2：执行超时
        /// </summary>
        [JsonProperty("failStatus")]
        public long? FailStatus { get; set; }
        /// <summary>
        /// 成功或失败信息
        /// </summary>
        [JsonProperty("result")]
        public string? Result { get; set; }
    }
}
