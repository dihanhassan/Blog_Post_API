using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPost.Domain.Entities
{
    public class PostCategory: BaseEntity
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }

        public Category Category { get; set; }
        public Post Post { get; set; }

    }
}
