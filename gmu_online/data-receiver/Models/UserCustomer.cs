namespace data_receiver.Models
{
    public class UserCustomer
    {
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int customerId { get; set; }
        public Customer customer { get; set; }
    }
}
