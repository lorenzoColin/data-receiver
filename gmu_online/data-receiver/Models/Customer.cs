using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string company { get; set; }
        public string? admin { get; set; }
        [Required]
        public string phonenumber { get; set; }
        [Required]
        public string city { get; set; }
        [Required]
        public string adress { get; set; }
        public int? actionId { get; set; }
        public action? action { get; set; }
        public virtual ICollection<CustomerContact>? CustomerContact { get; set; }
        public virtual ICollection<UserCustomer>? UserCustomer { get; set; }

     

      
    }

}
