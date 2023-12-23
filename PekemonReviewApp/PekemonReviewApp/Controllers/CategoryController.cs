using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PekemonReviewApp.Models;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository categoryRepository,IMapper mapper)
        {
            _categoryRepository= categoryRepository;
            _mapper=mapper;
        }
        [HttpGet]
        [ProducesResponseType(200, Type =typeof(IEnumerable<Category>))]
        public IActionResult GetCategoryList()
        {
            var categoryList=_mapper.Map<List<CategoryDto>>(_categoryRepository.GetCategories()); 
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(categoryList);    
        }

        [HttpGet("{catId}")]
        [ProducesResponseType(200, Type = typeof(Category))]
        public IActionResult GetCategory(int catId)
        {
            if (!_categoryRepository.CategoryExists(catId))
            {
                return BadRequest();
            }
            var categoryId = _mapper.Map<CategoryDto>(_categoryRepository.GetCategory(catId));
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return Ok(categoryId);
        }

        [HttpGet("{categoryId}/pokemon")]
        [ProducesResponseType(200,Type = typeof(IEnumerable<Pokemon>))]
        public IActionResult GetPokemonByCategoryId(int categoryId)
        {
            var pokemon = _mapper.Map<List<PokemonDto>>
                (_categoryRepository.GetPokemonByCategory(categoryId));

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            return Ok(pokemon);
        }

        [HttpPost]
        [ProducesResponseType(204)]
        public IActionResult CreateCategory([FromBody] CategoryDto categoryDto)
        {
            if(categoryDto == null)
            {
                return BadRequest(ModelState);
            }

            var category = _categoryRepository.GetCategories()
                .Where(c => c.Name.Trim().ToUpper() == 
                categoryDto.Name.Trim().ToUpper()).FirstOrDefault();

            if(category!=null)
            {
                ModelState.AddModelError("", "Category already exists!");
                return StatusCode(422, ModelState);
            }

            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var categoryMap = _mapper.Map<Category>(categoryDto);
            if (!_categoryRepository.CreateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while save in!");
                return StatusCode(500, ModelState);
            }

             return Ok("Successfully created!");
          
        }

        [HttpPut("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public IActionResult UpdateCategory(int categoryId, [FromBody] CategoryDto categoryDto)
        {
            if (categoryDto == null)
            {
                return BadRequest(ModelState);  
            }

            if (categoryId != categoryDto.Id)
            {
                return BadRequest(ModelState);
            }
            
            if(!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest();    
            }

            var categoryMap=_mapper.Map<Category>(categoryDto);
            if (!_categoryRepository.UpdateCategory(categoryMap))
            {
                ModelState.AddModelError("", "Somthing went wrong while save in!");
                return StatusCode(500, ModelState) ;    
            }
            return Ok("Successfully update!");
        }

        [HttpDelete("{categoryId}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public IActionResult DeleteCategory(int categoryId)
        {
            if (!_categoryRepository.CategoryExists(categoryId))
            {
                return NotFound();
            }

            var categoryDelete=_categoryRepository.GetCategory(categoryId); 
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if(!_categoryRepository.DeleteCategory(categoryDelete)) 
            {
                ModelState.AddModelError("", "Something went wrong deleting category!");
                return StatusCode(500,ModelState);
            }
            return NoContent();
        }
    }
}
