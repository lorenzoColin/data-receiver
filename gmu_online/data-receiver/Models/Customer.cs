using System.ComponentModel.DataAnnotations;

namespace data_receiver.Models
{
    public class Customer
    {
        public string _id { get; set; }

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
        public string? Latest_videocall { get; set; } 
        public string? Latest_contact { get; set; } = "n.v.t";
        public string? Remarks { get; set; } = "n.v.t";
        public string? CMS { get; set; } = "n.v.t";

        //propperty from sma_klanten

        public string? Servicefee_afspraak { get; set; } = "n.v.t";
        public string? CustomerType { get; set; } = "n.v.t";
        public string? status { get; set; } = "n.v.t";






    }

}
