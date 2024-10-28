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
            CategoryResponse res = await _categoryService.AddCategory(categoryRequest, 1);
            return Ok(res);
        }
        #endregion SAVE

        #region Update
        [HttpPut]
        [Route("category")]
        [Authorize]
        public async Task<IActionResult> UpdateCategory([FromBody] CategoryRequest categoryRequest, int id)
        {
            CategoryResponse res = await _categoryService.UpdateCategory(categoryRequest, id);
            return Ok(res);
        }

        #endregion Update

        #region Delete
        [HttpDelete]
        [Route("category")]
        [Authorize]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            CategoryResponse res = await _categoryService.DeleteCategory(id);
            return Ok(res);
        }
        #endregion Delete
    }
}
