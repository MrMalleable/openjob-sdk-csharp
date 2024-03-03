namespace openjob_sdk_csharp_agent.common.response
{
    /// <summary>
    /// 上线客户端接口的返回响应实体类
    /// </summary>
    public class OpenjobApiClusterStartResponse
    {
        /// <summary>
        /// 应用编号ID
        /// </summary>
        public long? AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        public string? AppName { get; set; }

        /// <summary>
        /// 应用所有在线客户端
        /// </summary>
        public IList<string>? WorkAddressList { get; set; }
    }
}
