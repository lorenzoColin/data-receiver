namespace data_receiver.Models.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            users = new Dictionary<string,string>();
        }
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        //get a user list that belongs to this role
        public Dictionary<string,string> users { get; set; }
    }

}
