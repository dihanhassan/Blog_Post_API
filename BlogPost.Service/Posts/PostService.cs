using AutoMapper;
using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;
using BlogPost.Application.Interfaces.Posts;
using BlogPost.Domain.Entities;
using BlogPost.Domain.Interfaces;
using BlogPost.Domain.Interfaces.Posts;

namespace BlogPost.Service.Posts
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IMapper _mapper;
        private readonly ITransactionUtil _transaction;
        public PostService(IPostRepository postRepository, IMapper mapper, ITransactionUtil transaction)
        {
            _postRepository = postRepository;
            _mapper = mapper;
            _transaction = transaction;
        }

        public async Task<PostResponse> AddPosts(PostRequest post, int AuthorId)
        {
            try
            {
                await _transaction.BeginAsync();
                Post PostEntity = _mapper.Map<Post>(post);
                Post res = await _postRepository.AddAsync(PostEntity);
                await _postRepository.SaveChangesAsync();
                await _transaction.CommitAsync();
                return _mapper.Map<PostResponse>(res);

            }
            catch (Exception ex)
            {
                await _transaction.RollBackAsync();
                throw;
            }
            
        }

        public async Task<PostResponse> DeletePosts(int id)
        {
            Post? deletedEntity = await _postRepository.GetByIdAsync(id);
            //await _postRepository.DeleteAsync(deletedEntity);
            await _postRepository.SoftDeleteAsync(deletedEntity);
            return _mapper.Map<PostResponse>(deletedEntity);
        }

        public Task<PostResponse> UpdatePosts(PostRequest post, int id)
        {
            throw new NotImplementedException();
        }
    }
}
