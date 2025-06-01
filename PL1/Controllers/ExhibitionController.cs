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
    public class ExhibitionController : ControllerBase
    {
        private readonly IExhibitionService _exhibitionService;
        private readonly IMapper _mapper;

        public ExhibitionController(IExhibitionService exhibitionService, IMapper mapper)
        {
            _exhibitionService = exhibitionService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var exhibitions = _exhibitionService.GetAllExhibitions();
            var exhibitionDtos = _mapper.Map<IEnumerable<ExhibitionDto>>(exhibitions);
            return Ok(exhibitionDtos);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var exhibition = _exhibitionService.GetExhibition(id);
                var exhibitionDto = _mapper.Map<ExhibitionDto>(exhibition);
                return Ok(exhibitionDto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public IActionResult Create([FromBody] ExhibitionDto exhibitionDto)
        {
            try
            {
                var exhibitionModel = _mapper.Map<ExhibitionModel>(exhibitionDto);
                _exhibitionService.AddExhibition(exhibitionModel);
                return CreatedAtAction(nameof(GetById), new { id = exhibitionDto.ExhibitionId }, exhibitionDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ExhibitionDto exhibitionDto)
        {
            try
            {
                if (id != exhibitionDto.ExhibitionId)
                    return BadRequest("ID in URL and body do not match");

                var exhibitionModel = _mapper.Map<ExhibitionModel>(exhibitionDto);
                _exhibitionService.UpdateExhibition(exhibitionModel);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
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
                _exhibitionService.DeleteExhibition(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }

        [HttpGet("current")]
        public IActionResult GetCurrentExhibitions()
        {
            var exhibitions = _exhibitionService.GetCurrentExhibitions();
            return Ok(_mapper.Map<IEnumerable<ExhibitionDto>>(exhibitions));
        }

        [HttpGet("search/theme/{theme}")]
        public IActionResult SearchByTheme(string theme)
        {
            var exhibitions = _exhibitionService.SearchExhibitionsByTheme(theme);
            return Ok(_mapper.Map<IEnumerable<ExhibitionDto>>(exhibitions));
        }
    }
}