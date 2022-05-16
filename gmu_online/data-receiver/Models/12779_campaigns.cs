using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace data_receiver.Models
{
    public class _12779_campaigns
    {   

        string _id { get; set; }
        [ JsonPropertyName("campaign.campaign_budget")]
        string campaign_campaign_budget { get; set; }
    }
}
