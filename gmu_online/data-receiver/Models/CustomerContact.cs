namespace data_receiver.Models
{
    public class CustomerContact
    {  
        public int customerId { get; set; }
        public Customer customer { get; set; }
        public int contactId { get; set; }
        public Contact contact { get; set; }
    }
}
