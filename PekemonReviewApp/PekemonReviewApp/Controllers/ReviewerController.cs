using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewerController : ControllerBase
    {
        private readonly IReviewerRepository _reviewerRepository;
        private readonly IMapper _mapper;
        public ReviewerController(IReviewerRepository reviewerRepository, IMapper mapper)
        {
            _reviewerRepository = reviewerRepository;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(200,Type=typeof(ICollection<Reviewer>))]
        public IActionResult GetReviewerList()
        {
            var reviewerList=_mapper.Map<List<ReviewerDto>>(_reviewerRepository.GetReviewers());
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);  
            }
            return Ok(reviewerList);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200,Type = typeof(Reviewer))]
        public IActionResult GetReviewer(int id)
        {
            if(!_reviewerRepository.ReviewerExist(id))
            {
                return BadRequest();
            }
            var reviewer = _mapper.Map<ReviewerDto>(_reviewerRepository.GetReviewer(id));
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(reviewer);

        }

        [HttpGet("{reviewerId}/reviewer")]
        [ProducesResponseType(200, Type =typeof(ICollection<Review>))]
        public IActionResult GetReview(int reviewerId)
        {
            var review=_mapper.Map<List<ReviewDto>>(_reviewerRepository.GetReviewsByReviewer(reviewerId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(review);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        public IActionResult CreateReviewer([FromBody]ReviewerDto reviewerDto)
        {
            if(reviewerDto==null)
            {
                return BadRequest(ModelState);
            }

            var reviewer=_reviewerRepository.GetReviewers().Where(r=>r.Lastname.Trim().ToUpper()
            ==reviewerDto.Lastname.Trim().ToUpper()).FirstOrDefault();
            if (reviewer != null)
            {
                ModelState.AddModelError("", "Reviewer already exists!");
                return StatusCode(422, ModelState);
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewerMap = _mapper.Map<Reviewer>(reviewerDto);

            if (!_reviewerRepository.CreateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while save in!");
                return StatusCode(500, ModelState);
            }
            return Ok("Successfully created!");
        }

        [HttpPut("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateReview(int reviewerId, [FromBody] ReviewerDto reviewerDto)
        {
            if (reviewerDto == null)
            {
                return BadRequest(ModelState);
            }
            if (reviewerId != reviewerDto.Id)
            {
                return BadRequest(ModelState);
            }
            if (!_reviewerRepository.ReviewerExist(reviewerId))
            {
                return NotFound();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var reviewerMap = _mapper.Map<Reviewer>(reviewerDto);
            if (!_reviewerRepository.UpdateReviewer(reviewerMap))
            {
                ModelState.AddModelError("", "Update Problem!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }

        [HttpDelete("{reviewerId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteReviewer(int reviewerId)
        {
            if (!_reviewerRepository.ReviewerExist(reviewerId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var ownerdelete = _reviewerRepository.GetReviewer(reviewerId);
            if (!_reviewerRepository.DeleteReviewer(ownerdelete))
            {
                ModelState.AddModelError("", "Something went wrong deleting reviewer!");
                return StatusCode(500, ModelState);
            }
            return NoContent();
        }
    }
}
