using MagicVilla_VillaAPI.data;
using MagicVilla_VillaAPI.logging;
using MagicVilla_VillaAPI.model;
using MagicVilla_VillaAPI.model.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaApiController : ControllerBase
    {
        private readonly ILogging _logger;

        public VillaApiController(ILogging logger) {
            _logger = logger;
        }


        [HttpGet]
        public ActionResult <IEnumerable<VillaDTO>> GetVillas()
        {
            return  Ok(VillaStore.vilaList);
        }  
        
        //[HttpGet("id")]
        [HttpGet("{id:int}",Name = "GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]   
       // [ProducesResponseType(200)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(400)]
        public ActionResult<VillaDTO> GetVilla(int id)
        {
            if (id==0)
            {
                _logger.Log("BAD REQUEST HERE IN GETTING VILLA...","Error");
                return BadRequest();
            }
            var villa = VillaStore.vilaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }

            return Ok(VillaStore.vilaList.FirstOrDefault(u=>u.Id==id));
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] //helps us to fix undocumented issue
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public ActionResult<VillaDTO> CreateVilla([FromBody]VillaDTO villaDTO)
        {

            //if we want  to add validation from model at the time of we didn't use APIController
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (VillaStore.vilaList.FirstOrDefault(u => u.Name.ToLower() == villaDTO.Name.ToLower()) != null)
            {
                //the key shoulld be unique.
                ModelState.AddModelError("Name", "Name already exists");
                return BadRequest(ModelState);
            }

            if (villaDTO== null)
            {
                return BadRequest(villaDTO);
            }
            if (villaDTO.Id > 0)
            {
                //for sending the custom status code.
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            villaDTO.Id = VillaStore.vilaList.OrderByDescending(u => u.Id).FirstOrDefault().Id + 1;
            VillaStore.vilaList.Add(villaDTO);
            //return Ok(villaDTO);
            return CreatedAtRoute("GetVilla", new {id=villaDTO.Id },villaDTO);
        }
        [HttpDelete("{id:int}",Name ="DeleteVilla")]

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteVilla(int id)
        {
            if (id == 0)
            {
                return BadRequest("there is some thing you missed buddy");
            }
            var villa = VillaStore.vilaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            VillaStore.vilaList.Remove(villa);
            return NoContent();
        }

        [HttpPut("{id:int}",Name ="UpdateVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult UpdatVilla(int id, [FromBody]VillaDTO villaDTO)
        {
            if (villaDTO == null || id != villaDTO.Id)
            {
                return BadRequest();
            }
            var villa=VillaStore.vilaList.FirstOrDefault(u=>u.Id == id);
            if (villa == null)
            {
                return NotFound();
            }
            villa.Name=villaDTO.Name;
            villa.Occupancy = villaDTO.Occupancy;
            villa.Sqft = villaDTO.Sqft;

            return NoContent();
        }

        [HttpPatch("{id:int}", Name = "UpdatePartialVilla")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public IActionResult UpdatePartialVilla(int id,JsonPatchDocument<VillaDTO> partialDTO)
        {

            if(id==0 || partialDTO == null)
            {
                return BadRequest();
            }

            var villa = VillaStore.vilaList.FirstOrDefault(u => u.Id == id);
            if (villa == null)
            {
                return BadRequest();
            }
            partialDTO.ApplyTo(villa, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();

        }
    }
}
