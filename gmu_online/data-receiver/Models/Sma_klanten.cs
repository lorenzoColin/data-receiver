namespace data_receiver.Models
{
    //this class only serves as retreving data
    //and pass the filled props to the customer class
    public class Sma_klanten
    {
            public string Debiteurnr { get; set; }
            public string Klant { get; set; }
            public string Beheerder { get; set; }
            public string Status { get; set; }
            public string datum_live { get; set; }

            public string budget_afspr { get; set; }

            public string Doelstelling { get; set; }

            public string Servicefee_afspraak { get; set; }

    }
}
