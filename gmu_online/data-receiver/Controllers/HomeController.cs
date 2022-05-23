using data_receiver.Models;
using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Http.Extensions;
using data_receiver.ElasticConnection;
using Nest;
using data_receiver.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using data_receiver.Models.ViewModels;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V10.Services;
using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.V10.Errors;
using Google.Api.Ads.AdWords.Lib;
using Google.Ads.Gax.Config;
using static Google.Ads.GoogleAds.V10.Resources.AccountBudget;


//"_id": "13249fa0-ad1f-4d17-8d36-7289503bb652",
//"Debiteurnr": "12759",
//"Klant": "Veiligheids-sloten",
//"Beheerder": "Annerieke ",
//"Status": "Live",
//"datum_live": "01-03-2021",
//"budget_afspr": "€ 5.000,00",
//"Doelstelling": "ROAS 1200%",
//"Servicefee_afspraak": "min. €450 >€500 staffel"
//},


namespace data_receiver.Controllers
{
    
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ElasticClient _client;
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;




        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {

            _client = new ElasticSearchClient().EsClient();
            _logger = logger;
            _db = db;
            _userManager = userManager;
        }

        public async Task< IActionResult> Index([FromServices] IFluentEmail singleEmai)
        {

            var list = new List<double>();

            


            var adswordconfig = new AdWordsAppConfig
            {
                DeveloperToken = "yBzwour2WR3cjnO6--vr7w",
                OAuth2ClientId = "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",
                OAuth2RefreshToken = "1//09rPlYi19UE5fCgYIARAAGAkSNwF-L9Ir7pKSkmp7OUsROXQn0KisD4l_0_wdtaS8HifT59R6z1_wDlthtZdeyZI1kUP5lVRhcMQ",
                ClientCustomerId = "3743262179",
                OAuth2ClientSecret = "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"
            };


            GoogleAdsConfig config = new GoogleAdsConfig()
            {
                DeveloperToken = "yBzwour2WR3cjnO6--vr7w",
                LoginCustomerId = "3743262179",
                OAuth2Mode = OAuth2Flow.APPLICATION,
                OAuth2ClientId = "515940014204-5o7gipoadae7vg70khnrk2qijrs7mmf9.apps.googleusercontent.com",
                OAuth2ClientSecret = "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",
                OAuth2RefreshToken = "1//09rXVRXziweZDCgYIARAAGAkSNwF-L9IrRfxuLR6OlDxxSyRa8umTp7fULugMkbZ-7beWPhb0X5wU81LqMZ3J7mVWYg-uFPW6-6E",
            };
            //connectie api
            var client = new GoogleAdsClient(config);
            var googleAdsService = client.GetService(Services.V10.GoogleAdsService);


            string query = @"SELECT
  campaign.name, 
  campaign.status,  
 metrics.cost_micros, metrics.average_cpc, metrics.average_cpm
FROM campaign
WHERE segments.date = '2022-05-23' AND campaign.name = '[NL] - Experience Center Cruquius - Radius Targeting'";


            //cost_micros nope
            // metrics.average_cost nope
            //metrics.current_model_attributed_conversions_value_per_cost // nope
            //metrics.active_view_measurable_cost_micros // nope 
            //metrics.average_cost  //nope 


            try
            {
                // Issue a search request.
                googleAdsService.SearchStream("7247702932", query,
                    delegate (SearchGoogleAdsStreamResponse resp)
                    {
                        foreach (var googleAdsRow in resp.Results)
                        {
                        Console.WriteLine(String.Format("campaigne name {0}| campaign status {1} | cost is {2} | cost {3}",googleAdsRow.Campaign.Name, googleAdsRow.Campaign.Status,googleAdsRow.Metrics.CostMicros / 1000000,googleAdsRow.Metrics.CostMicros ));


                        }
                    }
                );
            }
            catch (GoogleAdsException e)
            {
                Console.WriteLine("Failure:");
                Console.WriteLine($"Message: {e.Message}");
                Console.WriteLine($"Failure: {e.Failure}");
                Console.WriteLine($"Request ID: {e.RequestId}");
                throw;
            }

           var sss =  list.Sum() ;

            return View();
        }

        

        //[Authorize(Roles ="admin")]
        public async Task<IActionResult> Privacy([FromServices] IFluentEmail singleEmail)
        { 
            try
            {
                var email = singleEmail
               .To("lorenzo8399@hotmail.com")
               .Subject("Test email")
           .UsingTemplate("hi @Model.Name this is the first email @(5 + 5)!", new { Name = "test name" });
                await email.SendAsync();
                return Ok();
            }
            catch(Exception ex)
            {


                return BadRequest(ex.Message);
            }

        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        { 
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

  

}