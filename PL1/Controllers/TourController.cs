using Microsoft.AspNetCore.Mvc;
using Museum.BLL.Services;
using AutoMapper;
using Dtos.DTO;
using Museum.BLL.Models;

namespace Museum.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TourController : ControllerBase
    {
        private readonly TourService _tourService;
        private readonly IMapper _mapper;

        public TourController(TourService tourService, IMapper mapper)
        {
            _tourService = tourService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll([FromQuery] bool includeExhibitions = false)
        {
            var tours = _tourService.GetAllTours(includeExhibitions);
            return Ok(_mapper.Map<IEnumerable<TourDto>>(tours));
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id, [FromQuery] bool eagerLoading = false)
        {
            try
            {
                var tour = _tourService.GetTour(id, eagerLoading);
                return Ok(_mapper.Map<TourDto>(tour));
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("group")]
        public IActionResult CreateGroupTour([FromBody] TourDto tourDto)
        {
            try
            {
                var tourModel = _mapper.Map<TourModel>(tourDto);
                _tourService.PlanGroupTour(tourModel);
                return CreatedAtAction(nameof(GetById), new { id = tourDto.TourId }, tourDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("private")]
        public IActionResult CreatePrivateTour([FromBody] TourDto tourDto)
        {
            try
            {
                var tourModel = _mapper.Map<TourModel>(tourDto);
                _tourService.PlanPrivateTour(tourModel);
                return CreatedAtAction(nameof(GetById), new { id = tourDto.TourId }, tourDto);
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
                _tourService.DeleteTour(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("guide/{guideName}")]
        public IActionResult GetByGuide(string guideName)
        {
            var tours = _tourService.SearchToursByGuide(guideName);
            return Ok(_mapper.Map<IEnumerable<TourDto>>(tours));
        }
    }
}