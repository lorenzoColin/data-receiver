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

           





            //12779_campaigns
            // var template = @"@{ Layout = ""Views/Shared/MailTemplate.cshtml""; }";
            // var model = new { Name = "LUKE", Numbers = new[] { "1", "2", "3" } };
            // var email = singleEmai
            //    .To("lorenzo8399@hotmail.com")
            //    .Subject("Test email")
            //.UsingTemplate(template,model);
            // await email.SendAsync();

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