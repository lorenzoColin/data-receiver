using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class Customer
    {

        //props for sea_klanten
        [Key]
        public string? Debiteurnr { get; set; }
        public string? Klant { get; set; }
        public string? Consultant { get; set; }
        public string? Max_budget { get; set; }
        public string? Doelstelling { get; set; }
        public string? Resultaat { get; set; }
        public string? Datum_live { get; set; }
        public string? Contract { get; set; }
        public string? Contact { get; set; }
        public string? Latest_videocall { get; set; }
        public string? Latest_contact { get; set; }
        public string? Remarks { get; set; }
        public string? CMS { get; set; }

        //propperty from sma_klanten

        public string? Servicefee_afspraak { get; set; }
        public string? CustomerType { get; set; } 

        



       
    }

}
