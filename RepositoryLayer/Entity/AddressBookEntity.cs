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
        [Required, Phone, MaxLength(15)]
        public string ContactNumber { get; set; }
        [EmailAddress, MaxLength(255)]
        public string Email { get; set; }
        public string Address { get; set; }
        // Foreign Key
        public int UserId { get; set; }

        //Navigation Property
        [ForeignKey("UserId")]
        public UserEntity Users { get; set; }
    }
}
