using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class customer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string firstname { get; set; }
        [Required]
        public string lastname { get; set; }
        [Required]
        public string phonenumber { get; set; }
        [Required]
        public string company { get; set; }
        [Required]
        public string adress { get; set; }
        [Required]
        public string city { get; set; }
        public virtual ICollection<UserCustomer>? UserCustomer { get; set; }
        public Action? action { get; set; }

    }
}
