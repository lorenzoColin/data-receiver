using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class CustomerContact
    { 
        [Key]
        public int contactId { get; set; }
        public Contact contact { get; set; }
        public string customerId { get; set; }
    }
}
