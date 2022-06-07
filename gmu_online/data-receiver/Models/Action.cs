using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class action
    {
        [Key]
        public int id { get; set; }
        public string actionName { get; set; }
        public string description { get; set; }
        public virtual ICollection<UserCustomerAction> UserCustomerAction { get; set; }


    }
}
