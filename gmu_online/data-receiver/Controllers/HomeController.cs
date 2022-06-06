using data_receiver.Models;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using data_receiver.ElasticConnection;
using Nest;
using data_receiver.Data;
using Microsoft.AspNetCore.Identity;
using data_receiver.google_ads;
using Facebook.ApiClient.ApiEngine;

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
           //var userid = _userManager.GetUserId(HttpContext.User);

           ////google ads
           //var google = new GoogleAds();
           //var customerListt =  google.customerList();


           // //elastic data
           //var customer = new CustomerList(_db);
           //var list = customer.getallcustomers();


           // //uit deze lijst moet ik de naam uit alle klanten 
           //var customerwithAds = list.Where(s => s.customer.Ads != "this custoner does not have a ads yet");

           // foreach (var customerwithAd in customerwithAds)
           // {
           //     //get the customer account with id
           //     //trim omdat er spaties in de namen zitten
           //     string value = customerwithAd.customer.Ads.Trim();

           //     var customerlist = customerListt.Where(s => s.accountName == value).FirstOrDefault();

           //     if(customerlist != null)
           //     {
           //         //get the cost of this specific customer
           //         var cost = google.getcustomerWithCost(customerlist.accountId.ToString()).Sum();

           //         var roundCost = Math.Round(cost,2);
           //     }
           // }
            //seh.Where(s => s.accountName == );            

            //var cost = google.getcustomerWithCost("");

            var user = _userManager.GetUserId(HttpContext.User);
            var loggedInUser = _db.Users.Find(user);

            //customer that hase no relationship with the current users
            var customerList = new CustomerList(_db);
            var unclaimedCustomerlist = customerList.unclaimedCustomerlist(user).Take(5).ToList();

            ViewBag.claimedusers = customerList.claimedcustomerlist(user).Take(5).ToList();

            return View(unclaimedCustomerlist);
        }

        public async Task<ActionResult> claimCustomer(string id, string customerType)
        {
            //my user id 
            var userid = _userManager.GetUserId(HttpContext.User);
            //ingelogde user
            var loggedInUser = await _db.Users.FindAsync(userid);

            var userCustomer = new UserCustomer { DebiteurnrId = id, userid = userid, customerType = customerType };

            _db.UserCustomer.Add(userCustomer);
            _db.SaveChanges();

            return RedirectToAction("index");
        }


        public ActionResult removecustomer(string id, string CustomerType)
        {

            var loggedInId = _userManager.GetUserId(HttpContext.User);
            var customer = _db.UserCustomer.Where(x => x.userid == loggedInId && x.DebiteurnrId == id && x.customerType == CustomerType).First();

            _db.UserCustomer.Remove(customer);
            _db.SaveChanges();

            return RedirectToAction("index");
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