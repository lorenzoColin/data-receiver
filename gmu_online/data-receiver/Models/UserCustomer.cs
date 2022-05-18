using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class UserCustomer
    {
        [Key]
        public int Id { get; set; }
        public string userid { get; set; }
        public string DebiteurnrId { get; set; }

        public string customerType { get; set; }

        public virtual List<UserCustomerAction> UserCustomerAction { get; set; }

    }
}
