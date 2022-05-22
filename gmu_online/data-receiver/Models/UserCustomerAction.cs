using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class UserCustomerAction
    {
        [Key]
        public int id { get; set; }
        public int usercustomerId { get; set; }
        public UserCustomer? UserCustomer { get; set; }
        public int actionId { get; set; }
        public action? action { get; set; }
        public int value { get; set; }


    }

}
