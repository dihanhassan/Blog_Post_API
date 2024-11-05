using BlogPost.Data;
using BlogPost.Domain.Entities;
using BlogPost.Domain.Interfaces.Categories;
using Microsoft.EntityFrameworkCore;

namespace BlogPost.Repo.Categories
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly EFDbContext _context;
        public CategoryRepository(EFDbContext context) : base(context)
        {
            _context = context;
        }

        #region VarifyCategory
        public async Task<List<int>> VerifyCategory(List<int> categoryIds)
        {

            List<int> existingCategoryIds = await _context.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();

            List<int> notPresentCategoryIds = categoryIds.Except(existingCategoryIds).ToList();

            return notPresentCategoryIds;
        }
        #endregion VarifyCategory

    }
}
