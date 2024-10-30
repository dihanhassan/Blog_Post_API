﻿using AutoMapper;
using BlogPost.Application.CustomExceptions;
using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;
using BlogPost.Application.Interfaces.Categories;
using BlogPost.Domain.Entities;
using BlogPost.Domain.Interfaces.Categories;

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
        #endregion Private
        #region Save
        public async Task<CategoryResponse> AddCategory(CategoryRequest categoryRequest, int createdBy)
        {
            Category categoryEntity = _mapper.Map<Category>(categoryRequest);
            await _categoryRepository.AddAsync(categoryEntity);
            await _categoryRepository.SaveChangesAsync();
            CategoryResponse res = _mapper.Map<CategoryResponse>(categoryEntity);
            return res;
        }
        #endregion Save

        #region Delete
        public async Task<CategoryResponse> DeleteCategory(int id)
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
            await _categoryRepository.DeleteAsync(findTagetedCategoryEntity);
            await _categoryRepository.SaveChangesAsync();
            CategoryResponse deletedCategoryResponse = _mapper.Map<CategoryResponse>(findTagetedCategoryEntity);
            return deletedCategoryResponse;

        }
        #endregion Delete

        public Task<CategoryResponse> GetAllCategories()
        {
            throw new NotImplementedException();
        }

        public Task<CategoryResponse> GetCategory(int id)
        {
            throw new NotImplementedException();
        }
        #region Update
        public async Task<CategoryResponse> UpdateCategory(CategoryRequest categoryRequest, int id)
        {
            Category res = await ValidateCategoryUpdateRequest(id);
            res.Name = categoryRequest.Name ?? res.Name;
            res.Route = categoryRequest.Route ?? res.Route;
            await _categoryRepository.UpdateAsync(res);
            await _categoryRepository.SaveChangesAsync();
            CategoryResponse response = _mapper.Map<CategoryResponse>(res);
            return response;

        }
        #endregion Update
    }
}