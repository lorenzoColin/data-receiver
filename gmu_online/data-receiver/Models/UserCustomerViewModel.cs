using Microsoft.AspNetCore.Mvc.Rendering;

namespace data_receiver.Models
{
    public class UserCustomerViewModel
    {

        public int customerId{ set; get; }
        //hier maak ik customers dat van type seleclistItem is
        public List<SelectListItem>? customers { set; get; }

        public string userId { set; get; }
        //hier maak ik customers dat van type seleclistItem is
        public List<SelectListItem>? users { set; get; }

    }
}
