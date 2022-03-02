using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class Action
    {
        public string id { get; set; }

        public string name { get; set; }
        public TypeOfAction type { get; set; }
        public enum TypeOfAction
        {
            [Display(Name = "email.")]
            email,
            [Display(Name = "sms.")]
            sms, 
        }
    }
}
