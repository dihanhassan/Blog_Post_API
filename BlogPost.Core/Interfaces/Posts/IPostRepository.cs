using BlogPost.Domain.Entities;

namespace BlogPost.Domain.Interfaces.Posts
{
    public interface IPostRepository : IBaseRepository<Post>
    {
        Task<List<Post>> GetAllPostsByCategory(int id);
    }
}
