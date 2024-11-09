using BlogPost.Domain.Entities;

namespace BlogPost.Domain.Interfaces.Categories
{
    public interface ICategoryRepository : IBaseRepository<Category>
    {
        Task<List<int>> VerifyCategory(List<int> categoryIds);
    }
}
