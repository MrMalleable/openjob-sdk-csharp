namespace openjob_sdk_csharp_agent.common.response
{
    /// <summary>
    /// 请求返回相应实体类
    /// </summary>
    public class OpenjobApiResponse<T>
    {
        /// <summary>
        /// 具体业务数据
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// 非 200 请求失败
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 默认为0
        /// </summary>
        public int? Code { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 当前毫秒时间
        /// </summary>
        public long? SeverTime { get; set; }


    }
}
