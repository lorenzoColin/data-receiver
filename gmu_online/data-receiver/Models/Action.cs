namespace data_receiver.Models
{
    public class action
    {
        public int id { get; set; }
        public string name { get; set; }
        public virtual ICollection<UserCustomerAction> UserCustomerAction { get; set; }


    }
}
