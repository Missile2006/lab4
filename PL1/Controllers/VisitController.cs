using Microsoft.AspNetCore.Mvc;
using Museum.BLL.Services;
using AutoMapper;
using Dtos.DTO;
using Museum.BLL.Models;
using Museum.BLL.Interfaces;

namespace Museum.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VisitController : ControllerBase
    {
        private readonly IVisitService _visitService;
        private readonly IMapper _mapper;

        public VisitController(IVisitService visitService, IMapper mapper)
        {
            _visitService = visitService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var visits = _visitService.GetAllVisits();
            return Ok(_mapper.Map<IEnumerable<VisitDto>>(visits));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var visit = _visitService.GetVisit(id);
                return Ok(_mapper.Map<VisitDto>(visit));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] VisitDto visitDto)
        {
            try
            {
                var visitModel = _mapper.Map<VisitModel>(visitDto);
                _visitService.AddVisit(visitModel);
                return CreatedAtAction(nameof(GetById), new { id = visitDto.VisitId }, visitDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _visitService.DeleteVisit(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("search")]
        public IActionResult Search([FromQuery] string? visitorName, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            if (!string.IsNullOrEmpty(visitorName))
            {
                var visits = _visitService.SearchVisitsByName(visitorName);
                return Ok(_mapper.Map<IEnumerable<VisitDto>>(visits));
            }

            if (startDate.HasValue && endDate.HasValue)
            {
                var visits = _visitService.SearchVisitsByDateRange(startDate.Value, endDate.Value);
                return Ok(_mapper.Map<IEnumerable<VisitDto>>(visits));
            }

            return BadRequest("Provide either visitorName or date range");
        }
    }
}