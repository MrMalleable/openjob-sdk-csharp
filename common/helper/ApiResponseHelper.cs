using openjob_sdk_csharp_agent.common.response;

namespace openjob_sdk_csharp_agent.common.helper
{
    /// <summary>
    /// 返回响应工具类
    /// </summary>
    public static class ApiResponseHelper<T>
    {
        /// <summary>
        /// 构造成功响应
        /// </summary>
        public static OpenjobApiResponse<T> BuildSuccessResponse(T data)
        {
            return new OpenjobApiResponse<T>
            {
                Status = 200,
                Data = data,
                Code = 0,
                Message = "OK",
                ServerTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }

        /// <summary>
        /// 构造失败响应
        /// </summary>
        public static OpenjobApiResponse<T> BuildFailResponse(int status, int code, string message)
        {
            return new OpenjobApiResponse<T>
            {
                Status = status,
                Code = code,
                Message = message,
                ServerTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
        }
    }
}
