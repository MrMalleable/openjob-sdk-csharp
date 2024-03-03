using openjob_sdk_csharp_agent.common.response;

namespace openjob_sdk_csharp_agent.common.helper
{
    /// <summary>
    /// 返回响应工具类
    /// </summary>
    public class ApiResponseHelper<T>
    {
        /// <summary>
        /// 构造成功响应
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static OpenjobApiResponse<T> BuildSuccessResponse(T data)
        {
            return new OpenjobApiResponse<T>
            {
                Status = 200,
                Data = data,
                Code = 0,
                Message = "OK",
                SeverTime = DateTime.Now.Ticks
            };  
        }

        /// <summary>
        /// 构造失败响应
        /// </summary>
        /// <param name="status"></param>
        /// <param name="code"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static OpenjobApiResponse<T> BuildFailResponse(int status, int code, string message)
        {
            return new OpenjobApiResponse<T>
            {
                Status = status,
                Code = code,
                Message = message,
                SeverTime = DateTime.Now.Ticks
            };
        }
    }
}
