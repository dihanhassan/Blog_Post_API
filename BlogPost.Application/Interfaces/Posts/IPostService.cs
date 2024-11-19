using BlogPost.Application.Dto.Request;
using BlogPost.Application.Dto.Response;

namespace BlogPost.Application.Interfaces.Posts
{
    public interface IPostService
    {
        Task<ResponseDto<PostResponse>> AddPosts(PostRequest post, int AuthorId);
        Task<ResponseDto<PostResponse>> UpdatePosts(PostRequest post, int PostId);
        Task<ResponseDto<PostResponse>> DeletePosts(int id);
        Task<ResponseDto<List<PostResponse>>> GetAllPosts();
        Task<ResponseDto<PostResponse>> GetPost(int id);
    }
}
