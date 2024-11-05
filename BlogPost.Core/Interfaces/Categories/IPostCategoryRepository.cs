using BlogPost.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPost.Domain.Interfaces.Categories
{
    public interface IPostCategoryRepository : IBaseRepository<PostCategory>
    {
    }
}
