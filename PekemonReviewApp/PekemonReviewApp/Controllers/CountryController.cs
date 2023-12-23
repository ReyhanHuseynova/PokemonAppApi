using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PekemonReviewApp.DAL;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper;
        public CountryController(ICountryRepository countryRepository, IMapper mapper)
        {
            _countryRepository = countryRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type =typeof(IEnumerable<Country>))]
        public IActionResult GetCountryList()
        {
            var countryList=_mapper.Map<List<CountryDto>>(_countryRepository.GetCountryList());
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }
            return Ok(countryList);
        }

        [HttpGet("{countryId}")]
        [ProducesResponseType(200, Type =typeof(Country))]
        public IActionResult GetCountry(int countryId)
        {
            if (!_countryRepository.CountryExist(countryId))
            {
                return BadRequest();  
            } 
            var getCountry=_mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(getCountry);
        }

        [HttpGet("{countryId}/owner")]
        [ProducesResponseType(200, Type =(typeof(Country)))]
        public IActionResult GetCountryOwner(int countryId)
        {
            var country=_mapper.Map<CountryDto>(_countryRepository.GetCountry(countryId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(country == null)
            {
                return NotFound();
            }
            return Ok(country);

        }

        [HttpGet("{ownerId}/country")]
        [ProducesResponseType(200,Type =typeof(IEnumerable<Owner>))] 
        public IActionResult GetOwner(int ownerId)
        {
            var owner= _mapper.Map<List<OwnerDto>>(_countryRepository.GetOwnersFromACountry(ownerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState) ; 
            }
            if(owner == null)
            {
                return NotFound();
            }
            return Ok(owner);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        public IActionResult CreateCountry([FromBody] CountryDto countryDto)
        {
            if(countryDto == null)
            {
                return BadRequest(ModelState);
            }

            var country = _countryRepository.GetCountryList().Where(c => c.Name.Trim().ToUpper()
            == countryDto.Name.Trim().ToUpper()).FirstOrDefault();

            if (country != null)
            {
                ModelState.AddModelError("", "Country already exists!");
                return StatusCode(422, ModelState);
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }

            var countryMap = _mapper.Map<Country>(countryDto);
            if(!_countryRepository.CreateCountry(countryMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while save in!");
                return StatusCode(500,ModelState);
            }

            return Ok("Successfully created!");
        }

        [HttpPut("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int countryId, [FromBody] CountryDto cDto)
        {
            if (cDto == null)
            {
                return BadRequest(ModelState);
            }

            if (countryId != cDto.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_countryRepository.CountryExist(countryId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var cMap = _mapper.Map<Country>(cDto);
            if (!_countryRepository.UpdateCountry(cMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while save in!");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully update!");
        }

        [HttpDelete("{countryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCountry(int countryId)
        {
            if (!_countryRepository.CountryExist(countryId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var countryDelete=_countryRepository.GetCountry(countryId);
            if (!_countryRepository.DeleteCountry(countryDelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting country!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
