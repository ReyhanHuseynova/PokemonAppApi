using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PekemonReviewApp.DAL;
using PekemonReviewApp.Models;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        public OwnerController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _mapper = mapper;
            _countryRepository = countryRepository;
        }
        [HttpGet]
        [ProducesResponseType(200,Type =typeof(ICollection<Owner>))]
        public IActionResult GetOwnerList()
        {
            var owner= _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(owner);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200,Type=typeof(Owner))]  
        public IActionResult GetOwner(int ownerId)
        {
            if(!_ownerRepository.OwnerExists(ownerId)) 
            {
                return BadRequest();
            }

            var owner=_mapper.Map<OwnerDto>(_ownerRepository.GetOwner(ownerId));    
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(owner);
        }

        [HttpGet("{pokeId}/pokemon")]
        [ProducesResponseType(200,Type=typeof(ICollection<Owner>))] 
        public IActionResult GetOwnerPokemon(int pokeId)
        {
            var pokemon=_mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwnerOfAPokemon(pokeId));
            if (pokemon == null)
            {
                return NotFound();  
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemon);
        }

        [HttpGet("{ownerId}/owner")]
        [ProducesResponseType(200, Type =typeof(ICollection<Pokemon>))]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            var pokemonOwner=_mapper.Map<List<PokemonDto>>(_ownerRepository.GetPokemonByOwner(ownerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (pokemonOwner == null)
            {
                return NotFound();  
            }
            return Ok(pokemonOwner);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        public IActionResult Createowner([FromQuery] int countryId,[FromBody] OwnerDto ownerDto)
        {
            if (ownerDto == null)
            {
                return BadRequest(ModelState);
            }

            var owner=_ownerRepository.GetOwners().Where(o=>o.LastName.Trim().ToUpper()
            == ownerDto.LastName.Trim().ToUpper()).FirstOrDefault();     
            
            if (owner != null)
            {
                ModelState.AddModelError("", "Owner already exists!");
                return StatusCode(422,ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ownerMap = _mapper.Map<Owner>(ownerDto);
            ownerMap.Country=_countryRepository.GetCountry(countryId); 
            
            if (!_ownerRepository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while save in!");
                return StatusCode(500,ModelState);  
            }
            return Ok("Successfully created!");
        }


        [HttpPut("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int ownerId, [FromBody] OwnerDto oDto)
        {
            if (oDto == null)
            {
                return BadRequest(ModelState);
            }

            if (ownerId != oDto.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.CountryExist(ownerId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var oMap = _mapper.Map<Owner>(oDto);
            if (!_ownerRepository.UpdateOwner(oMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while save in!");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully update!");
        }

        [HttpDelete("{ownerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteOwner(int ownerId)
        {
            if (!_ownerRepository.OwnerExists(ownerId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ownerdelete = _ownerRepository.GetOwner(ownerId);
            if (!_ownerRepository.DeleteOwner(ownerdelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting owner!");
                return StatusCode(500, ModelState); 
            }
            return NoContent();
        }
    }
}
