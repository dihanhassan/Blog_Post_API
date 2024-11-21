using BlogPost.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogPost.Application.Dto.Response
{
    public class PostCategoryResponse : BaseEntity
    {
        public int Id { get; set; } = 0;
        public int CategoryId { get; set; } = 0;
        public int PostId { get; set; } = 0;
    }
}
