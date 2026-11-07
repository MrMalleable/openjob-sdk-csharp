using Microsoft.AspNetCore.Mvc;
using openjob_sdk_csharp_agent.common.helper;
using openjob_sdk_csharp_agent.common.request.agent;
using openjob_sdk_csharp_agent.core;

namespace openjob_sdk_csharp_agent.Controllers
{
    /// <summary>
    /// 定时任务接口：提供给 Server 回调
    /// </summary>
    [ApiController]
    [Route("job-instance")]
    public class JobInstanceController : ControllerBase
    {
        private readonly ILogger<JobInstanceController> _logger;
        private readonly JobInstanceService _jobInstanceService;

        public JobInstanceController(
            ILogger<JobInstanceController> logger,
            JobInstanceService jobInstanceService)
        {
            _logger = logger;
            _jobInstanceService = jobInstanceService;
        }

        /// <summary>
        /// 执行任务实例
        /// </summary>
        [HttpPost("submit")]
        public async Task<IActionResult> Submit([FromBody] AgentSubmitRequest request)
        {
            var (accepted, message) = await _jobInstanceService.SubmitAsync(request).ConfigureAwait(false);
            if (!accepted)
            {
                _logger.LogWarning("Submit rejected: {Message}", message);
                return Ok(ApiResponseHelper<string>.BuildFailResponse(500, 0, message));
            }

            return Ok(ApiResponseHelper<string>.BuildSuccessResponse(message));
        }

        /// <summary>
        /// 终止任务实例
        /// </summary>
        [HttpPost("stop")]
        public IActionResult Stop([FromBody] AgentStopRequest request)
        {
            var stopped = _jobInstanceService.Stop(request);
            if (!stopped)
            {
                return Ok(ApiResponseHelper<string>.BuildFailResponse(
                    500, 0, $"job instance not found: {request.JobInstanceId}"));
            }

            return Ok(ApiResponseHelper<string>.BuildSuccessResponse("stopped"));
        }

        /// <summary>
        /// 任务实例检查
        /// </summary>
        [HttpPost("check")]
        public IActionResult Check([FromBody] AgentCheckRequest request)
        {
            var running = _jobInstanceService.Check(request.JobInstanceId);
            return Ok(ApiResponseHelper<object>.BuildSuccessResponse(new
            {
                running,
                jobInstanceId = request.JobInstanceId
            }));
        }
    }
}
