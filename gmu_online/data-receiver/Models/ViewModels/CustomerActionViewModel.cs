namespace data_receiver.Models.ViewModels
{
    public class CustomerActionViewModel
    {

        public string CustomerId { get; set; }
        public int?  actionId { get; set; }

        public string actionName { get; set; } = "no action set";

        public Customer customer { get; set; }
    }
}
