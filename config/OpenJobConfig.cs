using openjob_sdk_csharp_agent.common.helper;

namespace openjob_sdk_csharp_agent.config
{
    /// <summary>
    /// OpenJob Worker 配置
    /// </summary>
    public class OpenJobConfig
    {
        public const string SectionName = "OpenJob";

        /// <summary>
        /// 集群地址，例如 http://localhost:8080
        /// </summary>
        public string? ClusterHost { get; set; }

        /// <summary>
        /// 心跳间隔（秒）
        /// </summary>
        public int HeartBeatInterval { get; set; } = 5;

        /// <summary>
        /// 运行 IP
        /// </summary>
        public string? Host { get; set; }

        /// <summary>
        /// 启动端口
        /// </summary>
        public int Port { get; set; } = 25588;

        /// <summary>
        /// 应用名称
        /// </summary>
        public string? AppName { get; set; }

        /// <summary>
        /// 应用 ID（注册成功后由 Server 回填）
        /// </summary>
        public long? AppId { get; set; }

        /// <summary>
        /// Worker 版本
        /// </summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>
        /// 请求 Token
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Worker 唯一标识
        /// </summary>
        public string? WorkerKey { get; set; }

        /// <summary>
        /// 协议类型，默认 http
        /// </summary>
        public string ProtocolType { get; set; } = "http";

        /// <summary>
        /// HTTP 请求超时（秒）
        /// </summary>
        public int HttpTimeoutSeconds { get; set; } = 10;

        /// <summary>
        /// Worker 地址，格式 IP:PORT
        /// </summary>
        public string Address => $"{Host}:{Port}";

        /// <summary>
        /// 补齐默认值并校验关键配置
        /// </summary>
        public void Normalize()
        {
            if (HeartBeatInterval <= 0)
            {
                HeartBeatInterval = 5;
            }

            if (Port <= 0)
            {
                Port = 25588;
            }

            if (HttpTimeoutSeconds <= 0)
            {
                HttpTimeoutSeconds = 10;
            }

            if (string.IsNullOrWhiteSpace(Host))
            {
                Host = NetHelper.GetLocalIp();
            }

            if (string.IsNullOrWhiteSpace(Version))
            {
                Version = "1.0.0";
            }

            if (string.IsNullOrWhiteSpace(WorkerKey))
            {
                WorkerKey = $"{Environment.MachineName}-{Guid.NewGuid():N}";
            }

            if (string.IsNullOrWhiteSpace(ProtocolType))
            {
                ProtocolType = "http";
            }

            ClusterHost = ClusterHost?.TrimEnd('/');
        }

        /// <summary>
        /// 返回配置错误信息；为空表示可用
        /// </summary>
        public string? Validate()
        {
            if (string.IsNullOrWhiteSpace(ClusterHost))
            {
                return "ClusterHost is not configured";
            }

            if (string.IsNullOrWhiteSpace(AppName))
            {
                return "AppName is not configured";
            }

            if (string.IsNullOrWhiteSpace(Token))
            {
                return "Token is not configured";
            }

            if (string.IsNullOrWhiteSpace(Host))
            {
                return "Host is empty and local IPv4 cannot be resolved";
            }

            return null;
        }
    }
}
