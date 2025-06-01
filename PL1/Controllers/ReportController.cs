using Microsoft.AspNetCore.Mvc;
using Museum.BLL.Interfaces;
using Museum.BLL.Services;

namespace Museum.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;

        public ReportController(IReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("current-exhibitions")]
        public IActionResult GetCurrentExhibitionsReport()
        {
            var report = _reportService.GetCurrentExhibitionsReport();
            return Ok(report);
        }

        [HttpGet("visits")]
        public IActionResult GenerateVisitsReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var report = _reportService.GenerateVisitsReport(startDate, endDate);
                return Ok(report);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("tours")]
        public IActionResult GenerateToursReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var report = _reportService.GenerateToursReport(startDate, endDate);
                return Ok(report);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("popular-exhibitions")]
        public IActionResult GeneratePopularExhibitionsReport([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var report = _reportService.GeneratePopularExhibitionsReport(startDate, endDate);
                return Ok(report);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}