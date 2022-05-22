namespace data_receiver.Models.ViewModels
{
    //new model met // 1.live_clients 2. usercustomerId 3.Usercustomeraction model 4. live_clients type customer

    public class UserCustomerActionViewModel
    {
        public Customer customer { get; set; }
        public int usercustomerId { get; set;}
        public IEnumerable<UserCustomerAction> UserCustomerAction { get; set;}

        public IEnumerable<action>  action { get; set; }
    }
}
