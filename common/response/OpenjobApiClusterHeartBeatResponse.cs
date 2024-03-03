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
        public long? clusterVersion { get; set; }

        /// <summary>
        /// 集群延时任务版本号
        /// </summary>
        public long? clusterDelayVersion { get; set; }

        /// <summary>
        /// 应用所有在线客户端(包含当前客户端)
        /// </summary>
        public IList<string>? WorkAddressList { get; set; }
    }
}
