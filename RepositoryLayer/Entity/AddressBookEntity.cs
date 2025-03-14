using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace RepositoryLayer.Entity
{
    public class AddressBookEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string ContactName { get; set; }
        [Required]
        public string ContactNumber { get; set; }
        // Foreign Key
        public int UserId { get; set; }

        //Navigation Property
        [ForeignKey("UserId")]
        public UserEntity Users { get; set; }
    }
}
