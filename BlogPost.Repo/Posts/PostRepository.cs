using BlogPost.Data;
using BlogPost.Domain.Entities;
using BlogPost.Domain.Interfaces.Posts;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Repo.Posts
{
    public class PostRepository : BaseRepository<Post>, IPostRepository
    {
        private readonly EFDbContext _context;
        public PostRepository(EFDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Post>> GetAllPostsByCategory(int id)
        {
            return await _context.Posts
           .FromSqlRaw("SELECT p.* FROM dbo.Post p INNER JOIN dbo.PostCategories pc ON p.Id = pc.PostId WHERE pc.CategoryId = {0}", id)
           .ToListAsync();
        }
    }
}
