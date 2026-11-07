using Newtonsoft.Json;

namespace openjob_sdk_csharp_agent.common.response
{
    /// <summary>
    /// 请求返回响应实体类，对齐 OpenJob Result
    /// </summary>
    public class OpenjobApiResponse<T>
    {
        /// <summary>
        /// 具体业务数据
        /// </summary>
        [JsonProperty("data")]
        public T? Data { get; set; }

        /// <summary>
        /// 非 200 请求失败
        /// </summary>
        [JsonProperty("status")]
        public int? Status { get; set; }

        /// <summary>
        /// 默认为0
        /// </summary>
        [JsonProperty("code")]
        public int? Code { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty("message")]
        public string? Message { get; set; }

        /// <summary>
        /// 当前毫秒时间
        /// </summary>
        [JsonProperty("serverTime")]
        public long? ServerTime { get; set; }
    }
}
