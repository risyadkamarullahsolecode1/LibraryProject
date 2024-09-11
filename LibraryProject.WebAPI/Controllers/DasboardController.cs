using LibraryProject.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LibraryProject.WebAPI.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    public class DasboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;

        public DasboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetKpiReport()
        {
            var result = await _dashboardService.GetReport();
            return Ok(result);
        }
    }
}
