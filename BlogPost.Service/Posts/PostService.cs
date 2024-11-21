using AutoMapper;
using BlogPost.Application.CustomExceptions;
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
        private readonly IAuthRepository _authRepository;
        public PostService(

            IPostRepository postRepository, IMapper mapper,
            ITransactionUtil transaction,
            IPostCategoryRepository postCategoryRepository,
            IAuthRepository authRepository
        )
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _transaction = transaction;
            _postCategoryRepository = postCategoryRepository;
            _authRepository = authRepository;
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

        private async Task<Post> ValidatePostUpdateRequest(int id)
        {
            try
            {
                Post? post = await _postRepository.GetByIdAsync(id);
                if (post == null)
                {
                    throw new ClientCustomException("Post not found", new()
                    {
                        {"Id", "Post Id is not valid." }
                    });
                }
                return await Task.FromResult(post);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<string> GetAutherName(int AuthorId)
        {
            try
            {
                User? user = await _authRepository.GetByIdAsync(AuthorId);
                if (user == null)
                {
                    throw new ClientCustomException("User not found", new()
                    {
                        {"Id", "User Id is not valid." }
                    });
                }
                string name = $"{user.FirstName} {user.LastName}";
                return await Task.FromResult(name);
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
                string? AutherName = await GetAutherName(AuthorId); 
                PostEntity.CreatedBy = AutherName;
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

        public async Task<ResponseDto<List<PostResponse>>> GetAllPostByCategory(int id)
        {
            try
            {
                var postList = await _postRepository.GetAllPostsByCategory(id);
                var postRes = _mapper.Map<List<PostResponse>>(postList);
                return await ServiceHelper.MapToResponse(postRes, "Post fetched successfully");
            }
            catch(Exception)
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

        public async Task<ResponseDto<PostResponse>> UpdatePosts(PostRequest postreq, int id)
        {
            try
            {
                // can mapping problem
                Post? post = await ValidatePostUpdateRequest(id);
                Post postEntity = _mapper.Map<Post>(postreq);
                postEntity.Id = id;
                await _postRepository.UpdateAsync(postEntity);
                await _postRepository.SaveChangesAsync();
                var postRes = _mapper.Map<PostResponse>(postEntity);
                return await ServiceHelper.MapToResponse(postRes, "Post updated successfully");
            }
            catch (Exception)
            {
                throw;
            }
            throw new NotImplementedException();
        }
    }
}
