using Newtonsoft.Json;

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
        [JsonProperty("appId")]
        public long? AppId { get; set; }

        /// <summary>
        /// 应用名称
        /// </summary>
        [JsonProperty("appName")]
        public string? AppName { get; set; }

        /// <summary>
        /// 应用所有在线客户端
        /// </summary>
        [JsonProperty("workerAddressList")]
        public IList<string>? WorkerAddressList { get; set; }
    }
}
