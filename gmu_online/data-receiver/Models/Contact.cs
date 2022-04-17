using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class Contact
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string email { get; set; }
        [Required]
        public string firstname { get; set; }
        [Required]
        public string lastname { get; set; }
        [Required]
        public string phonenumber { get; set; }

        [Required]
        public string adress { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        public string birthdate { get; set; }
        public virtual ICollection<CustomerContact>? CustomerContact { get; set; }

    }
}
