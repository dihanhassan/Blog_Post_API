using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;

namespace BlogPost.Application.Interfaces.Categories
{
    public interface ICategoryService
    {
        #region Get
        Task<ResponseDto<CategoryResponse>> GetCategory(int id);
        Task<ResponseDto<List<CategoryResponse>>> GetAllCategories();
        Task<ResponseDto<List<PostCategoryResponse>>> GetAllPostCategory();
        #endregion Get

        #region Save
        Task<ResponseDto<CategoryResponse>> AddCategory(CategoryRequest categoryRequest, int createdBy);
        #endregion Save

        #region Update
        Task<ResponseDto<CategoryResponse>> UpdateCategory(CategoryRequest categoryRequest, int id);
        #endregion Update

        #region Delete
        Task<ResponseDto<CategoryResponse>> DeleteCategory(int id);
        #endregion Delete

    }
}
