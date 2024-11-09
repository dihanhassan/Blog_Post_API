using BlogPost.Data;
using BlogPost.Domain.Entities;
using BlogPost.Domain.Interfaces.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPost.Repo.PostCategories
{
    public class PostCategoryRepository: BaseRepository<PostCategory>, IPostCategoryRepository
    {
        private readonly EFDbContext _context;
        public PostCategoryRepository(EFDbContext context) : base(context)
        {
            _context = context;
        }

    }
}
