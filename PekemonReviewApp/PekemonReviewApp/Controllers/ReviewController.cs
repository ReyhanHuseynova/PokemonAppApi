using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IPokemonRepository _pokemonRepository;
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;
        public ReviewController(IReviewRepository reviewRepository, IReviewerRepository reviewerRepository, IPokemonRepository pokemonRepository, IMapper mapper)
        {
            _reviewRepository = reviewRepository;
            _mapper = mapper;
            _pokemonRepository = pokemonRepository;
            _reviewerRepository = reviewerRepository;
        }
        [HttpGet]
        [ProducesResponseType(200,Type =typeof(ICollection<Review>))]
        public IActionResult GetReviewList()
        {
            var reviewList= _mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviews());
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviewList);

        }

        [HttpGet("{id}")]
        [ProducesResponseType(200,Type =typeof(Review))]
        public IActionResult GetReview(int id)
        {
            if(!_reviewRepository.ReviewExist(id))
            {
                return NotFound();
            }
            var review=_mapper.Map<ReviewDto>(_reviewRepository.GetReview(id));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(review);
        }

        [HttpGet("{pokeId}/review")]
        [ProducesResponseType(200, Type =typeof(ICollection<Review>))]
        public IActionResult GetReviewPokemon(int pokeId)
        {
            var reviewPokemon=_mapper.Map<List<ReviewDto>>(_reviewRepository.GetReviewOfAPokemon(pokeId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (reviewPokemon == null)
            {
                return NotFound();
            }
            return Ok(reviewPokemon);

        }

        [HttpPost]
        [ProducesResponseType(204)]
        public IActionResult CreateReview([FromQuery] int pokemonId, [FromQuery] int revId, [FromBody]ReviewDto reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest(ModelState);  
            }

            var review = _reviewRepository.GetReviews().Where(r => r.Title.Trim().ToUpper()
            == reviewDto.Title.Trim().ToUpper()).FirstOrDefault();

            if(review != null)
            {
                ModelState.AddModelError("", "Review title already exists!");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewMap = _mapper.Map<Review>(reviewDto);
            reviewMap.Pokemon = _pokemonRepository.GetPokemon(pokemonId);
            reviewMap.Reviewer = _reviewerRepository.GetReviewer(revId);

            if(!_reviewRepository.CreateReview(reviewMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while save in!");
                return StatusCode(500,ModelState);  
            }
            return Ok("Successfully created!");
        }

        [HttpPut("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReview(int reviewId, [FromBody] ReviewDto reviewDto)
        {
            if (reviewDto == null)
            {
                return BadRequest(ModelState);
            }
            if(reviewId != reviewDto.Id)
            {
                return BadRequest(ModelState);
            }
            if(!_reviewRepository.ReviewExist(reviewId))
            {
                return NotFound();  
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewMap=_mapper.Map<Review>(reviewDto);   
            if(!_reviewRepository.UpdateReview(reviewMap))
            {
                ModelState.AddModelError("", "Update Problem!");
                return StatusCode(500, ModelState); 
            }
            return NoContent(); 
        }


        [HttpDelete("{reviewId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReview(int reviewId)
        {
            if (!_reviewRepository.ReviewExist(reviewId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ownerdelete = _reviewRepository.GetReview(reviewId);
            if (!_reviewRepository.DeleteReview(ownerdelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting review!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
