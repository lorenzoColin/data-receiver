using Microsoft.AspNetCore.Mvc.Rendering;

namespace data_receiver.Models.ViewModels
{
    public class UserRoleViewModel
    {
        public string? RoleId { get; set; }
        public string? RoleName { get; set; } = "no roll yet";

        public ApplicationUser User { get; set; }

        public List<SelectListItem>? list {get;set;}


        public bool? InRole { get; set; }

    }
}
