namespace data_receiver.Models
{
    public class UserCustomer
    {
        public int customerId { get; set; }
        public customer customer { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
