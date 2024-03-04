using Microsoft.AspNetCore.Mvc;
using openjob_sdk_csharp_agent.common.helper;
using openjob_sdk_csharp_agent.common.request.agent;

namespace openjob_sdk_csharp_agent.Controllers
{
    /// <summary>
    /// 定时任务接口
    /// 描述：提供给Server进行调用
    /// </summary>
    [ApiController]
    [Route("/job-instance")]
    public class JobInstanceController : Controller
    {

        private readonly ILogger<JobInstanceController> _logger;

        public JobInstanceController(ILogger<JobInstanceController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// 执行任务实例接口
        /// </summary>
        /// <returns></returns>
        [HttpPost("/submit")]
        public JsonResult Submit(AgentSubmitRequest request)
        {
            return Json(ApiResponseHelper<string>.BuildSuccessResponse("执行任务实例"));
        }

        /// <summary>
        /// 终止任务实例接口
        /// </summary>
        /// <returns></returns>
        [HttpPost("/stop")]
        public JsonResult Stop(AgentStopRequest request)
        {
            return Json(ApiResponseHelper<string>.BuildSuccessResponse("终止任务实例"));
        }

        /// <summary>
        /// 任务实例检查接口
        /// </summary>
        /// <returns></returns>
        [HttpPost("/check")]
        public JsonResult Check(AgentCheckRequest request)
        {
            return Json(ApiResponseHelper<string>.BuildSuccessResponse("任务实例检查"));
        }
    }
}
