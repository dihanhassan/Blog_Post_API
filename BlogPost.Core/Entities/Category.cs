using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlogPost.Domain.Entities
{
    [Table("Categories", Schema = "dbo")]
    //[Index("Name", IsUnique = true, = "Category_Route")]
    public class Category : BaseEntity
    {
        public Category()
        {
            PostCategories = new HashSet<PostCategory>();
        }
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        [Required]
        [MaxLength(128)]
        [Column("Route")]
        public string? Route { get; set; }
        public virtual ICollection<PostCategory> PostCategories { get; set; }
    }
}
