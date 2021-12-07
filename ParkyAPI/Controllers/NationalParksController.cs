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
    [Route("api/v{version:apiVersion}/nationalParks")]

    //[Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(GroupName = "ParkyDocNP")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public class NationalParksController : ControllerBase
    {
        private readonly INationalParkRepository _npRepo;
        private readonly IMapper _mapper;

        public NationalParksController(INationalParkRepository npRepo, IMapper mapper)
        {
            _npRepo = npRepo;
            _mapper = mapper;
        }
        /// <summary>
        /// Get List Of National Parks.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(200,Type=typeof(List<NationalParkDto>))]
        [ProducesResponseType(404)]
        public IActionResult GetNationaParks()
        {
            var objlets = _npRepo.GetNationalParks();
            var objDto = new List<NationalParkDto>();

            foreach (var obj in objlets)
            {
                objDto.Add(_mapper.Map<NationalParkDto>(obj));
            }
            return Ok(objDto);
        }

        /// <summary>
        /// Get Individual Of National Parks.
        /// </summary>
        /// <param name="id"> The ID of National Park</param>
        /// <returns></returns>
        [HttpGet("{id:int}", Name = "GetNationaPark")]
        [ProducesResponseType(200, Type = typeof(NationalParkDto))]
        [ProducesResponseType(404)]
        [Authorize]
        [ProducesDefaultResponseType]
        public IActionResult GetNationaPark(int id)
        {
            var obj = _npRepo.GetNationalPark(id);
            if (obj == null) return NotFound("Sorry Not Founded");


            var objDto = _mapper.Map<NationalParkDto>(obj);
            /*  If i Dont Auto Mapper */
            //var manuallyObjDto  = new NationalParkDto()
            //{
            //    Id = obj.Id,
            //    Name = obj.Name,
            //    State = obj.State,
            //    Created = obj.Created,
            //    Picture = obj.Picture,
            //    Established = obj.Established
            //};

            return Ok(objDto);
        }
        
        [HttpPost]
        [ProducesResponseType(201, Type = typeof(NationalParkDto))]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult CreateNationaPark([FromBody] NationalParkDto nationalParkDto)
        {
            if (nationalParkDto == null) return BadRequest(ModelState);

            if (_npRepo.NationalParkExist(nationalParkDto.Name))
            {
                ModelState.AddModelError("Add Nationa Park Error", "Nationa Park Exists!");
                return StatusCode(404, ModelState);
            }

            var objDto = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_npRepo.CreateNationalPark(objDto))
            {
                ModelState.AddModelError("", $"Somting wrong when saving record{objDto.Name}");
                return StatusCode(500, ModelState);
            }

            return CreatedAtRoute("GetNationaPark", new { version = HttpContext.GetRequestedApiVersion().ToString() , id = objDto.Id }, objDto) ;
        }

        [HttpPatch("{id:int}",Name = "UpdateNationaPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesDefaultResponseType]
        public IActionResult UpdateNationaPark(int id, [FromBody] NationalParkDto nationalParkDto )
        {
            if (nationalParkDto == null||  id != nationalParkDto.Id) return BadRequest(ModelState);


            if (!_npRepo.NationalParkExist(nationalParkDto.Id))
            {
                ModelState.AddModelError("Update Nationa Park Error", "Nationa Park Not Exists!");
                return StatusCode(404, ModelState);
            }
            if (!ModelState.IsValid) return BadRequest(ModelState);


            var objDto = _mapper.Map<NationalPark>(nationalParkDto);

            if (!_npRepo.UpdateNationalPark(objDto))
            {
                ModelState.AddModelError("", $"Somting wrong when updating record{objDto.Name}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{id:int}", Name = "DeleteeNationaPark")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult DeleteeNationaPark(int id)
        {

            if (!_npRepo.NationalParkExist(id)) return NotFound();


            if (!_npRepo.DeleteNationalPark(id))
            {
                ModelState.AddModelError("", $"Deleting err National Park ID : {id}");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

    }
}
