using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;

namespace BlogPost.Application.Interfaces.Posts
{
    public interface IPostService
    {
        public Task<PostResponse> AddPosts(PostRequest post, int AuthorId);
        public Task<PostResponse> UpdatePosts(PostRequest post, int PostId);
        public Task<PostResponse> DeletePosts(int id);
        Task<List<PostResponse>> GetAllPosts();
    }
}
