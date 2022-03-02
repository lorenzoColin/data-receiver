using Microsoft.AspNetCore.Identity;

namespace data_receiver.Models
{
    //inherit the identityuser model
    //so i can add my own propperties
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<UserCustomer> UserCustomer { get; set;}
    }
}
