using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class Customer
    {
        

        //props for sea_klanten
        public string? Debiteurnr { get; set; } = "n.v.t";
        public string? Klant { get; set; } = "n.v.t";
        public string? Consultant { get; set; } = "n.v.t";
        public string? Max_budget { get; set; } = "n.v.t";
        public string? Doelstelling { get; set; } = "n.v.t";
        public string? Resultaat { get; set; } = "n.v.t";
        public string? Datum_live { get; set; } = "n.v.t";
        public string? Contract { get; set; } = "n.v.t";
        public string? Contact { get; set; } = "n.v.t";
        public string? Latest_videocall { get; set; } = "n.v.t";
        public string? Latest_contact { get; set; } = "n.v.t";
        public string? Remarks { get; set; } = "n.v.t";
        public string? CMS { get; set; } = "n.v.t";
        public string? Ads { get; set; } = "this custoner does not have a ads yet";

        //propperty from sma_klanten
        public string? Servicefee_afspraak { get; set; } = "n.v.t";
        public string? CustomerType { get; set; } = "n.v.t";
        //seo and sma
        public string? status { get; set; } = "n.v.t";

        //seo klanten
        public string? URL_klant { get; set; }
        public string? Service_fee { get; set; }
        public string? Overeenkomst { get; set; }

        









    }

}
