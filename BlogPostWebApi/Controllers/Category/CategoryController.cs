using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;
using BlogPost.Application.Interfaces.Categories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BlogPostWebApi.Controllers.Category
{
    [Route("api/category-management")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        #region Properties
        public readonly ICategoryService _categoryService;
        #endregion Properties

        #region ctor
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }
        #endregion ctor

        #region SAVE
        [HttpPost]
        [Route("category")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryResponse))]
        public async Task<IActionResult> AddCategory([FromBody] CategoryRequest categoryRequest)
        {
            
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
            var  res = await _categoryService.AddCategory(categoryRequest, Convert.ToInt32(userId));
            return Ok(res);
        }
        #endregion SAVE

        #region GET
        [HttpGet]
        [Route("category")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CategoryResponse>))]
        public async Task<IActionResult> GetALLCategory()
        {
            var res = await _categoryService.GetAllCategories();
            return Ok(res);
        }

        [HttpGet]
        [Route("category-by-id")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CategoryResponse))]
        public async Task<IActionResult> GetById( int id)
        {
            var res = await _categoryService.GetCategory(id);
            return Ok(res);
        }
        #endregion GET

        #region Update
        [HttpPut]
        [Route("category")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryRequest categoryRequest, int id)
        {
           var res = await _categoryService.UpdateCategory(categoryRequest, id);
            return Ok(res);
        }

        #endregion Update

        #region Delete
        [HttpDelete]
        [Route("category")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var res = await _categoryService.DeleteCategory(id);
            return Ok(res);
        }
        #endregion Delete
    }
}
