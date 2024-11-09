using BlogPost.Data;
using BlogPost.Domain.Entities;
using BlogPost.Domain.Interfaces.Posts;

namespace BlogPost.Repo.Posts
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        private readonly EFDbContext _context;
        public PostRepository(EFDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
