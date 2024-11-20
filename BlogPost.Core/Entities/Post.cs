using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPost.Domain.Entities
{
    [Table("Post", Schema = "dbo")]
    public class Post : BaseEntity
    {
        public Post()
        {
            PostCategories = new HashSet<PostCategory>();
        }

        [Key]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Route { get; set; }
        public string? Content { get; set; }
        public string? CreatedBy { get; set; }
        public virtual ICollection<PostCategory> PostCategories { get; set; }
    }
}
