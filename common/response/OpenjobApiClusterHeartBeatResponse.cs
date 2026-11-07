using Newtonsoft.Json;

namespace openjob_sdk_csharp_agent.common.response
{
    /// <summary>
    /// 客户端心跳接口的返回响应实体类
    /// </summary>
    public class OpenjobApiClusterHeartBeatResponse
    {
        /// <summary>
        /// 集群通信版本号
        /// </summary>
        [JsonProperty("clusterVersion")]
        public long? ClusterVersion { get; set; }

        /// <summary>
        /// 集群延时任务版本号
        /// </summary>
        [JsonProperty("clusterDelayVersion")]
        public long? ClusterDelayVersion { get; set; }

        /// <summary>
        /// 应用所有在线客户端(包含当前客户端)
        /// </summary>
        [JsonProperty("workerAddressList")]
        public IList<string>? WorkerAddressList { get; set; }
    }
}
