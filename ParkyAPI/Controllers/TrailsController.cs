using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkyAPI.Models;
using ParkyAPI.Models.DTOS;
using ParkyAPI.ParkyMapper;
using ParkyAPI.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkyAPI.Controllers
{
    [Route("api/v{version:apiVersion}/trails")]
    //[Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyDocTrails")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class TrailsController : ControllerBase
    {
        private readonly ITrailRepository _trailRepo;
        private readonly IMapper _mapper;

        public TrailsController(ITrailRepository trailRepo, IMapper mapper)
        {
            _trailRepo = trailRepo;
            _mapper = mapper;
        }
        /// <summary>
        /// Get List Of Trails.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200,Type=typeof(List<TrailDto>))]
        [ProducesResponseType(404)]
        public IActionResult GetTrails()
        {
            var objlets = _trailRepo.GetTrails();
            var objDto = new List<TrailDto>();

            foreach (var obj in objlets)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }
            return Ok(objDto);
        }

        /// <summary>
        /// Get Individual Of Trails.
        /// </summary>
        /// <param name="id"> The ID of Trail</param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "GetTrail")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [ProducesDefaultResponseType]

        [Authorize(Roles ="Admin")]
        public IActionResult GetTrail(int id)
        {
            var obj = _trailRepo.GetTrail(id);
            if (obj == null) return NotFound("Sorry Not Founded");


            var objDto = _mapper.Map<TrailDto>(obj);

            return Ok(objDto);
        }

        //[HttpGet("GetTrailInNationalPark/{nationalParkId:int}")]
        [HttpGet("[action]/{nationalParkId:int}")]
        [ProducesResponseType(200, Type = typeof(TrailDto))]
        [ProducesResponseType(404)]
        [Authorize]
        [ProducesDefaultResponseType]
        public IActionResult GetTrailInNationalPark(int nationalParkId)
        {
            var objList = _trailRepo.GetTrailsInNationalPark(nationalParkId);
            if (objList == null) return NotFound("Sorry Not Founded");
 
            var objDto = new List<TrailDto>();
            foreach (var obj in objList)
            {
                objDto.Add(_mapper.Map<TrailDto>(obj));
            }

            return Ok(objDto);
        }

        [HttpPost]
        [ProducesResponseType(201, Type = typeof(TrailDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult CreateTrail([FromBody] TrailCreateDto trailCreateDto)
        {
            if (trailCreateDto == null) return BadRequest(ModelState);
            if (_trailRepo.TrailExist(trailCreateDto.Name))
            {
                ModelState.AddModelError("Add Trail Error", "Trail Exists!");
                return StatusCode(404, ModelState);
            }



            var objDto = _mapper.Map<Trail>(trailCreateDto);

            if (!_trailRepo.CreateTrail(objDto))
            {
                ModelState.AddModelError("", $"Somting wrong when saving record{objDto.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetTrail", new { Id = objDto.Id }, objDto);
            //return Ok(); ;
        }

        [HttpPatch("{id:int}",Name = "UpdateTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult UpdateTrail(int id, [FromBody] TrailUpdateDto trailDto)
        {
            if (trailDto == null || id != trailDto.Id) return BadRequest(ModelState);


            if (!_trailRepo.TrailExist(trailDto.Id))
            {
                ModelState.AddModelError("Update Trial Error", "Trial Not Exists!");
                return StatusCode(404, ModelState);
            }


            var objDto = _mapper.Map<Trail>(trailDto);

            if (!_trailRepo.UpdateTrail(objDto))
            {
                ModelState.AddModelError("", $"Somting wrong when updating record{objDto.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteTrail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteeTrail(int id)
        {

            if (!_trailRepo.TrailExist(id)) return NotFound();


            if (!_trailRepo.DeleteTrail(id))
            {
                ModelState.AddModelError("", $"Deleting err Trail ID : {id}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}
