namespace data_receiver.Models.ViewModels
{
    public class AdminUserCustomerViewModel
    {

        public ApplicationUser User { get; set; }

        public List<Customer> Customer { get; set; }

        public string role { get; set; }
    }
}
