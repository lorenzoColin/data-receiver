using data_receiver.Models;
using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Mail;
using System.Text;

namespace data_receiver.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index() 
        { 

        
            return View();
        }

        //[Authorize(Roles ="admin")]
        public async Task<IActionResult> Privacy([FromServices] IFluentEmail singleEmail)
        {
            //StringBuilder template = new StringBuilder();

            //template.AppendLine("Dear @model.firstname");
            //template.AppendLine("<p>thanks for blablabla @model.productname. we hope you enjoy</p>");
            //var model = new { firstname = "LUKE", productname = "product 1"};




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