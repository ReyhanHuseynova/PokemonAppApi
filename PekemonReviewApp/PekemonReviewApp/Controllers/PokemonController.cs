using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PekemonReviewApp.Models;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PokemonController : ControllerBase
    {
        private readonly IPokemonRepository _repository;
        private readonly IMapper _mapper;
        public PokemonController(IPokemonRepository repository, IMapper mapper)
        {
            _repository=repository;
            _mapper=mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type =typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemonList()
        {
            var pokemon= _mapper.Map<List<PokemonDto>>(_repository.GetPokemons());
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }
            return Ok(pokemon);
        }

        [HttpGet("{pokeId}")]
        [ProducesResponseType(200, Type =typeof(Pokemon))]
        public IActionResult GetPokemon(int pokeId)
        {
            if (!_repository.PokemonExist(pokeId))
            {
                return BadRequest();
            }
            var pokemon= _mapper.Map<PokemonDto>(_repository.GetPokemon(pokeId));
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokemon);
        }

        [HttpGet("{pokeId}/rating")]
        [ProducesResponseType(200, Type =typeof(decimal))]
        public IActionResult RatingPokemon(int pokeId)
        {
            if (!_repository.PokemonExist(pokeId))
            {
                return BadRequest();
            }
            var pokeRating=_repository.GetPokemonRating(pokeId);
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(pokeRating);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        public IActionResult CreatePokemon([FromQuery]int ownerId,[FromQuery] int pokeId,[FromBody] PokemonDto pokemonDto)
        {
            if(pokemonDto == null)
            {
                return BadRequest(ModelState);
            }
            var pokemon=_repository.GetPokemons().Where(p=>p.Name.Trim().ToUpper()
            ==pokemonDto.Name.ToUpper()).FirstOrDefault();

            if(pokemon != null)
            {
                ModelState.AddModelError("", "Pokemon already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var pokemonMap = _mapper.Map<Pokemon>(pokemonDto);

            if (!_repository.CreatePokemon(ownerId,pokeId,pokemonMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while save in!");
                return StatusCode(500, ModelState);     
            }
            return Ok("Successfully created!");
        }

        [HttpPut("{pokeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int pokeId, [FromQuery] int ownerId,
            [FromQuery] int catId,[FromBody] PokemonDto pDto)
        {
            if (pDto == null)
            {
                return BadRequest(ModelState);
            }

            if (pokeId != pDto.Id)
            {
                return BadRequest(ModelState);
            }

            if (!_repository.PokemonExist(pokeId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var pMap = _mapper.Map<Pokemon>(pDto);
            if (!_repository.UpdatePokemon(ownerId,catId,pMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while save in!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{pokeId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int pokeId)
        {
            if (!_repository.PokemonExist(pokeId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ownerdelete = _repository.GetPokemon(pokeId);
            if (!_repository.DeletePokemon(ownerdelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting pokemon!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }



    }
}
