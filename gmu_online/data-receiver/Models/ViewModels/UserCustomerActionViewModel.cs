namespace data_receiver.Models.ViewModels
{
    //new model met // 1.live_clients 2. usercustomerId 3.Usercustomeraction model 4. live_clients type customer

    public class UserCustomerActionViewModel
    {
        public Customer live_clients { get; set; }
        public int usercusotmerId { get; set;}
        public IEnumerable<UserCustomerAction> UserCustomerAction { get; set;}

        public IEnumerable<action>  action { get; set; }
    }
}
