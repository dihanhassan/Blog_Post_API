using AutoMapper;
using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;
using BlogPost.Application.Interfaces.Posts;
using BlogPost.Domain.Entities;
using BlogPost.Domain.Interfaces;
using BlogPost.Domain.Interfaces.Categories;
using BlogPost.Domain.Interfaces.Posts;
using BlogPost.Service.Helper;

namespace BlogPost.Service.Posts
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IPostCategoryRepository _postCategoryRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionUtil _transaction;
        public PostService(

            IPostRepository postRepository, IMapper mapper,
            ITransactionUtil transaction,
            IPostCategoryRepository postCategoryRepository
        )
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _transaction = transaction;
            _postCategoryRepository = postCategoryRepository;
        }
        #region private
        private async Task AddPostCategoryAsync(List<int> categoryIds, int postId)
        {
            try
            {
                List<PostCategory> postCategoryList = [];
                foreach (var categoryId in categoryIds)
                {
                    PostCategory postCategory = new()
                    {
                        CategoryId = categoryId,
                        PostId = postId
                    };
                    postCategoryList.Add(postCategory);
                }
                await _postCategoryRepository.AddRangeAsync(postCategoryList);
                await _postCategoryRepository.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion private
        public async Task<ResponseDto<PostResponse>> AddPosts(PostRequest post, int AuthorId)
        {
            try
            {
                await _transaction.BeginAsync();
                Post PostEntity = _mapper.Map<Post>(post);
                Post res = await _postRepository.AddAsync(PostEntity);
                await _postRepository.SaveChangesAsync();
                await AddPostCategoryAsync(post.CategoryIds, res.Id);
                await _transaction.CommitAsync();
                var postRes = _mapper.Map<PostResponse>(res);
                return await ServiceHelper.MapToResponse(postRes, "Post added successfully");

            }
            catch (Exception)
            {
                await _transaction.RollBackAsync();
                throw;
            }
            
        }

        public async Task<ResponseDto<List<PostResponse>>> GetAllPosts()
        {
            try
            {
                var postList = await _postRepository.GetAllAsync();
                var postRes =  _mapper.Map<List<PostResponse>>(postList);
                return await ServiceHelper.MapToResponse(postRes, "Post fetched successfully");
            }
            catch(Exception)
            {
                throw;
            }
        }

        public async Task<ResponseDto<PostResponse>> GetPost(int id)
        {
            try
            {
                Post? post = await _postRepository.GetByIdAsync(id);
                var postRes = _mapper.Map<PostResponse>(post);
                return await ServiceHelper.MapToResponse(postRes, "Post fetched successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ResponseDto<PostResponse>> DeletePosts(int id)
        {
            try
            {
                Post? deletedEntity = await _postRepository.GetByIdAsync(id);
                //await _postRepository.DeleteAsync(deletedEntity);
                await _postRepository.SoftDeleteAsync(deletedEntity);
                var postRes = _mapper.Map<PostResponse>(deletedEntity);
                return await ServiceHelper.MapToResponse(postRes, "Post deleted successfully");
            }
            catch (Exception)
            {
                throw;
            }
        }

        public Task<ResponseDto<PostResponse>> UpdatePosts(PostRequest post, int id)
        {
            throw new NotImplementedException();
        }
    }
}
