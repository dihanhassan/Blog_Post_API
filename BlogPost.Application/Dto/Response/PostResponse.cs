using BlogPost.Domain.Entities;

namespace BlogPost.Application.Dto.Response
{
    public class PostResponse : BaseEntity
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? CreatedBy { get; set; }
    }
}
