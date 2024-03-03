using Newtonsoft.Json;

namespace openjob_sdk_csharp_agent.common.request
{
    /// <summary>
    /// 任务日志选项实体类
    /// </summary>
    public class UploadLogField
    {
        public UploadLogField(string name, string value)
        {
            Name = name;
            Value = value;
        }

        /// <summary>
        /// 日志选项名称
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }
    }
}
