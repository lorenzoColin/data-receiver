namespace data_receiver.Models.ViewModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            users = new List<string>();
        }
        public string RoleId { get; set; }
        public string RoleName { get; set; }

        //get a user list that belongs to this role
        public List<string> users { get; set; }
    }

}
