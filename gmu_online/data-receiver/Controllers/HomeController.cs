using data_receiver.Models;
using FluentEmail.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using data_receiver.ElasticConnection;
using Nest;
using data_receiver.Data;
using Microsoft.AspNetCore.Identity;
using data_receiver.google_ads;using Gobln.Pager;
using System;


using Facebook;
using FacebookAds;
using FacebookAds.Object;
using FacebookAds.Object.Fields;
using System.Dynamic;

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
            var user = _userManager.GetUserId(HttpContext.User);
            var loggedInUser = _db.Users.Find(user);
            //customer that hase no relationship with the current users
            var customerList = new CustomerList(_db);
            var unclaimedCustomerlist = customerList.unclaimedCustomerlist(user).Take(5).ToList();

            ViewBag.claimedusers = customerList.claimedcustomerlist(user).Take(5).ToList();
            return View(unclaimedCustomerlist);
        }

        public async Task<ActionResult> claimCustomer(string id, string customerType,string klant)
        {
            //my user id 
            var userid = _userManager.GetUserId(HttpContext.User);
            //ingelogde user
            var loggedInUser = await _db.Users.FindAsync(userid);
            var userCustomer = new UserCustomer { DebiteurnrId = id, userid = userid, customerType = customerType, Klant = klant };

            _db.UserCustomer.Add(userCustomer);
            _db.SaveChanges();

            return RedirectToAction("index");
        }


        public ActionResult removecustomer(string id, string CustomerType,string klant)
        {

            var loggedInId = _userManager.GetUserId(HttpContext.User);
            var customer = _db.UserCustomer.Where(x => x.userid == loggedInId && x.DebiteurnrId == id && x.customerType == CustomerType && x.Klant == klant ).First();

            _db.UserCustomer.Remove(customer);
            _db.SaveChanges();

            return RedirectToAction("index");
        }

        //[Route("home/privacy/{currentpage}")]
        //public async Task<IActionResult> Privacy(int currentpage = 1)
        //{

            //// Create an List oject
            //var list = new PagedList<object>()
            //{
            //    new { Id = 1, Name = "Tester1", Date = new DateTime( 2015, 5,1 ) },
            //    new  { Id = 2, Name = "Tester2", Date = new DateTime( 2015, 5,2 ) },
            //    new { Id = 3, Name = "Tester3", Date = new DateTime( 2015, 5,3 ) },
            //    new { Id = 4, Name = "Tester4", Date = new DateTime( 2015, 5,4 ) },
            //    new { Id = 5, Name = "Tester5", Date = new DateTime( 2015, 5,5 ) },
            //    new { Id = 6, Name = "Tester6", Date = new DateTime( 2015, 5,1 ) },
            //    new{ Id = 7, Name = "Tester7", Date = new DateTime( 2015, 5,2 ) },
            //    new { Id = 8, Name = "Tester8", Date = new DateTime( 2015, 5,3 ) },
            //    new { Id = 9, Name = "Tester9", Date = new DateTime( 2015, 5,4 ) },
            //    new { Id = 10, Name = "Tester10", Date = new DateTime( 2015, 5,5 ) },
            //};


            //// Set the page values, if not set default pageidex is 1 and size is 10
            //list.CurrentPageIndex = 1;
            //list.PageSize = 3;

            //var size = list.PageCount;


            //// Get the current page form the pagelist
            //var pager = list.GetCurrentPage();


            //var CurrentPageNumber = list.CurrentPageIndex;
            //// Get the page at index X
            //pager = list.GetPage(currentpage);

            //return View(list);
        //}


        //[Route("home/prvacy/test")]
        //public ActionResult privacy2()
        //{

        //    // Create an List oject
        //    var list = new PagedList<object>()
        //    {
        //        new { Id = 1, Name = "Tester1", Date = new DateTime( 2015, 5,1 ) },
        //        new  { Id = 2, Name = "Tester2", Date = new DateTime( 2015, 5,2 ) },
        //        new { Id = 3, Name = "Tester3", Date = new DateTime( 2015, 5,3 ) },
        //        new { Id = 4, Name = "Tester4", Date = new DateTime( 2015, 5,4 ) },
        //        new { Id = 5, Name = "Tester5", Date = new DateTime( 2015, 5,5 ) },
        //        new { Id = 6, Name = "Tester6", Date = new DateTime( 2015, 5,1 ) },
        //        new{ Id = 7, Name = "Tester7", Date = new DateTime( 2015, 5,2 ) },
        //        new { Id = 8, Name = "Tester8", Date = new DateTime( 2015, 5,3 ) },
        //        new { Id = 9, Name = "Tester9", Date = new DateTime( 2015, 5,4 ) },
        //        new { Id = 10, Name = "Tester10", Date = new DateTime( 2015, 5,5 ) },
        //    };


        //    return Json(list);
        //}
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        { 
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

  

}