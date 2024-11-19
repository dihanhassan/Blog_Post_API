using AutoMapper;
using BlogPost.Application.CustomExceptions;
using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;
using BlogPost.Application.Interfaces.Categories;
using BlogPost.Domain.Entities;
using BlogPost.Domain.Interfaces.Categories;
using BlogPost.Service.Helper;

namespace BlogPost.Service.Categories
{
    public class CategoryService : ICategoryService
    {
        public readonly ICategoryRepository _categoryRepository;
        public readonly IMapper _mapper;
        public CategoryService(ICategoryRepository categoryRepository, IMapper mapper)
        {
            _categoryRepository = categoryRepository;
            _mapper = mapper;
        }
        #region Private
        private async Task<Category> ValidateCategoryUpdateRequest(int id)
        {
            Category? existedCategory = await _categoryRepository.GetByIdAsync(id);
            if (existedCategory == null)
            {
                throw new ClientCustomException("Category not found", new()
                {
                    {"Id", "Category Id is not valid." }
                });
            }
            return existedCategory;

        }

        private async Task<bool> ExistingCategory(string route)
        {
            var existedCategory = _categoryRepository.GetByCondition(x => x.Route == route).FirstOrDefault();
            if (existedCategory != null)
            {
                throw new ClientCustomException("Category already exists", new()
                {
                    {"Route", "Category Route is already exists." }
                });
            }
            return true;
        }
        #endregion Private

        #region Save
        public async Task<ResponseDto<CategoryResponse>> AddCategory(CategoryRequest categoryRequest, int createdBy)
        {
            try
            {
                await ExistingCategory(categoryRequest.Route);
                Category categoryEntity = _mapper.Map<Category>(categoryRequest);
                await _categoryRepository.AddAsync(categoryEntity);
                await _categoryRepository.SaveChangesAsync();
                CategoryResponse res = _mapper.Map<CategoryResponse>(categoryEntity);
                return await ServiceHelper.MapToResponse(res,"Add Category Successfully.");
            }
            catch (Exception)
            {
                throw ;
            }
        }
        #endregion Save

        #region Delete
        public async Task<ResponseDto<CategoryResponse>> DeleteCategory(int id)
        {
            try
            {
                Category res = await ValidateCategoryUpdateRequest(id);
                Category? findTagetedCategoryEntity = await _categoryRepository.GetByIdAsync(id);
                if (findTagetedCategoryEntity is null)
                {
                    throw new ClientCustomException("Category not found", new()
                {
                    {"Id", "Category Id is not valid." }
                });
                }
                //var MappedResponse = _mapper.Map<Category>(findTagetedCategoryEntity);
                await _categoryRepository.SoftDeleteAsync(findTagetedCategoryEntity);
                await _categoryRepository.SaveChangesAsync();
                CategoryResponse deletedCategoryResponse = _mapper.Map<CategoryResponse>(findTagetedCategoryEntity);
                return await ServiceHelper.MapToResponse(deletedCategoryResponse,"Deleted Category Successfully.");
            }
            catch(Exception)
            {
                throw;
            }
        }
        #endregion Delete

        public async Task<ResponseDto<List<CategoryResponse>>> GetAllCategories()
        {
            try
            {
                var categories  = await _categoryRepository.GetAllAsync();
                var res = _mapper.Map<List<CategoryResponse>>(categories);
                return await ServiceHelper.MapToResponse(res,"Fetched all categories SuccessFully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseDto<CategoryResponse>> GetCategory(int id)
        {
            try
            {
                var category = await _categoryRepository.GetByIdAsync(id);
                var res = _mapper.Map<CategoryResponse>(category);
                return await ServiceHelper.MapToResponse(res, "Fetched Category Successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }
        #region Update
        public async Task<ResponseDto<CategoryResponse>> UpdateCategory(CategoryRequest categoryRequest, int id)
        {
            Category res = await ValidateCategoryUpdateRequest(id);
            res.Name = categoryRequest.Name ?? res.Name;
            res.Route = categoryRequest.Route ?? res.Route;
            await _categoryRepository.UpdateAsync(res);
            await _categoryRepository.SaveChangesAsync();
            CategoryResponse response = _mapper.Map<CategoryResponse>(res);
            return await ServiceHelper.MapToResponse(response,"Update Category Successfully");

        }
        #endregion Update
    }
}
