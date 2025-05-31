using Microsoft.AspNetCore.Mvc;
using Museum.BLL.Services;
using AutoMapper;
using Dtos.DTO;
using Museum.BLL.Models;

namespace Museum.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ScheduleController : ControllerBase
    {
        private readonly ScheduleService _scheduleService;
        private readonly IMapper _mapper;

        public ScheduleController(ScheduleService scheduleService, IMapper mapper)
        {
            _scheduleService = scheduleService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var schedules = _scheduleService.GetAllSchedules();
            return Ok(_mapper.Map<IEnumerable<ScheduleDto>>(schedules));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var schedule = _scheduleService.GetSchedule(id);
                return Ok(_mapper.Map<ScheduleDto>(schedule));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] ScheduleDto scheduleDto)
        {
            try
            {
                var scheduleModel = _mapper.Map<ScheduleModel>(scheduleDto);
                _scheduleService.AddSchedule(scheduleModel);
                return CreatedAtAction(nameof(GetById), new { id = scheduleDto.ScheduleId }, scheduleDto);
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
                _scheduleService.DeleteSchedule(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("exhibition/{exhibitionId}")]
        public IActionResult GetByExhibition(int exhibitionId)
        {
            var schedules = _scheduleService.GetExhibitionSchedules(exhibitionId);
            return Ok(_mapper.Map<IEnumerable<ScheduleDto>>(schedules));
        }

        [HttpGet("available/{date}")]
        public IActionResult GetAvailable(DateTime date)
        {
            var schedules = _scheduleService.GetAvailableSchedules(date);
            return Ok(_mapper.Map<IEnumerable<ScheduleDto>>(schedules));
        }
    }
}