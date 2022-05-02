using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class UserCustomer
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string? DebiteurnrId { get; set; }


        public ICollection<UserCustomerAction> UserCustomerAction { get; set; }


    }
}
