using data_receiver.Data;
using data_receiver.ElasticConnection;
using data_receiver.Models;
using data_receiver.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Nest;
using System.Text.Encodings;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.WebUtilities;
using System.Net;

namespace data_receiver.Controllers
{

    public class UserCustomerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly ElasticClient _client;
        public static string error;
        public static int errorCount;


        public UserCustomerController(ApplicationDbContext db, UserManager<ApplicationUser> usermanager)
        {
            _client = new ElasticSearchClient().EsClient();
            _usermanager = usermanager;
            _db = db;
        }
        // GET: UserCustomerController 
        public async Task<ActionResult> Index()
        {

            var userId = _usermanager.GetUserId(HttpContext.User);
            //ingelogde user
            var loggedInUser = await _db.Users.FindAsync(userId);

            var customerlist = new CustomerList(_db);

            var claimedcustomers = customerlist.claimedcustomerlist(userId);

            ViewBag.Actions = _db.action;



            return View(claimedcustomers);
        }


        [Route("usercustomer/edit/{DebiteurnrId}/customerType/{customerType}/klant/{klant}")]
        public async Task<ActionResult> Edit(string DebiteurnrId, string customerType, string klant)
        {



            var userId = _usermanager.GetUserId(HttpContext.User);
            TempData["error"] = error;

            if (TempData["error"] != null && errorCount > 1)
            {
                error = null;
                errorCount = 0;
            }

            //ingelogde user
            var loggedInUser = await _db.Users.FindAsync(userId);
            var customerlist = new CustomerList(_db);
            var mycustomers = customerlist.claimedcustomerlist(userId);




            var decodeuriklant = WebUtility.UrlDecode(klant).Trim();


            //containts have to become ==
            var Customer = mycustomers.Where(s => s.customer.CustomerType == customerType && s.customer.Debiteurnr == DebiteurnrId && s.customer.Klant.Contains(decodeuriklant)).FirstOrDefault();
            var UserCustomer = _db.UserCustomer.Where(s => s.userid == userId && s.DebiteurnrId == Customer.customer.Debiteurnr && s.customerType == Customer.customer.CustomerType && s.Klant == Customer.customer.Klant).FirstOrDefault();

            var usercustomerId = UserCustomer.Id;




            IEnumerable<UserCustomerAction> usercustomeraction = _db.UserCustomerAction.Where(s => s.usercustomerId == usercustomerId);

            var Action = _db.action;
            var UserCustomerActionViewModel = new UserCustomerActionViewModel { customer = Customer.customer, action = Action, usercustomerId = usercustomerId, UserCustomerAction = usercustomeraction };


            return View(UserCustomerActionViewModel);
        }

        public async Task< ActionResult> SetTriggerPopupModel(UserCustomerAction UserCustomerAction, string customerType ,string DebiteurnrId,string klant)
        {
            var userId = _usermanager.GetUserId(HttpContext.User);
            var customerlist = new CustomerList(_db);
            var claimedcustomerlist = customerlist.claimedcustomerlist(userId);
            var usercustomer = _db.UserCustomer.Where(s => s.userid == userId && s.customerType == customerType && s.Klant == klant && s.DebiteurnrId == DebiteurnrId && s.Klant == klant).FirstOrDefault();



            var userCustomerActions = _db.UserCustomerAction.Where<UserCustomerAction>(s => s.usercustomerId == usercustomer.Id);
           
            var duplicateUsercustomerAction = new List<UserCustomerAction>();
            bool duplicate = false;
            //loop over alle usercustomeraction die jij heb
            foreach (var userCustomerAction in userCustomerActions)
            {
                switch (userCustomerAction.actionId)
                {
                    //if actie id 1
                    case 1:
                        //if the value already existed
                        if (userCustomerAction.value == UserCustomerAction.value)
                        {
                            var sejjjjj = _db.UserCustomerAction.Find(userCustomerAction.id);
                            duplicateUsercustomerAction.Add(sejjjjj);
                            duplicate = true;
                        }
                        if (duplicate == true)
                        {
                            duplicateUsercustomerAction[0].value = UserCustomerAction.value;
                            _db.SaveChanges();
                            error = "duplicate value";
                            errorCount++;
                        }
                        break;
                    //last video call actionId 2
                    case 2:
                        if (userCustomerAction.value == UserCustomerAction.value)
                        {
                            var sejjjjj = _db.UserCustomerAction.Find(userCustomerAction.id);
                            duplicateUsercustomerAction.Add(sejjjjj);
                            duplicate = true;
                            break;
                        }
                        if (duplicate == true)
                        {
                            duplicateUsercustomerAction[0].value = UserCustomerAction.value;
                            _db.SaveChanges();
                            ModelState.AddModelError("duplicate", "duplicate value");
                        }
                        break;
                }
            }
            //if there is a duplicate overwrite the same value instead
            //actionId 1 == Currentbudget
            if (UserCustomerAction.actionId == 1 && duplicate == false)
            {
                UserCustomerAction.usercustomerId = usercustomer.Id;
                _db.UserCustomerAction.Add(UserCustomerAction);
                _db.SaveChanges();
            }
            //actionId == 2 Latest_videocall 
            if (UserCustomerAction.actionId == 2 && duplicate == false)
            {
                _db.UserCustomerAction.Add(UserCustomerAction);
                _db.SaveChanges();
            }


            if(usercustomer != null)
            {
                
                return RedirectToAction("index");
            }

            return RedirectToAction("index");

        }



        // POST: UserCustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(UserCustomerAction UserCustomerAction,string customerType, string klant)
        {
            var userCustomerActions = _db.UserCustomerAction.Where<UserCustomerAction>(s => s.usercustomerId == UserCustomerAction.usercustomerId && s.actionId == UserCustomerAction.actionId );
            var duplicateUsercustomerAction = new List<UserCustomerAction>();
            bool duplicate = false;
            //loop over alle usercustomeraction die jij heb
            foreach (var userCustomerAction in userCustomerActions)
            {
                switch (userCustomerAction.actionId)
                {
                    //if actie id 1() max budget
                    case 1:
                        //if the value already existed
                        if (userCustomerAction.value == UserCustomerAction.value)
                        {
                            var sejjjjj = _db.UserCustomerAction.Find(userCustomerAction.id);
                            duplicateUsercustomerAction.Add(sejjjjj);
                            duplicate = true;
                        }
                        if (duplicate == true)
                        {
                            duplicateUsercustomerAction[0].value = UserCustomerAction.value;
                            _db.SaveChanges();

                            error = "duplicate value";
                             errorCount++;
                        }
                        break;
                        //last video call actionId 2
                    case 2:
                        //if the value already existed
                        if (userCustomerAction.value == UserCustomerAction.value)
                        {
                            var sejjjjj = _db.UserCustomerAction.Find(userCustomerAction.id);
                            duplicateUsercustomerAction.Add(sejjjjj);
                            duplicate = true;
                        }
                        if (duplicate == true)
                        {
                            duplicateUsercustomerAction[0].value = UserCustomerAction.value;
                            _db.SaveChanges();

                            error = "duplicate value";
                            errorCount++;
                        }
                      break;
                    case 3:
                        //if the value already existed
                        if (userCustomerAction.value == UserCustomerAction.value)
                        {
                            var sejjjjj = _db.UserCustomerAction.Find(userCustomerAction.id);
                            duplicateUsercustomerAction.Add(sejjjjj);
                            duplicate = true;
                        }
                        if (duplicate == true)
                        {
                            duplicateUsercustomerAction[0].value = UserCustomerAction.value;
                            _db.SaveChanges();

                            error = "duplicate value";
                            errorCount++;
                        }
                        break;
                }
        }
            //if there is a duplicate overwrite the same value instead
            //actionId 1 == Currentbudget
            if (UserCustomerAction.actionId == 1 && duplicate == false)
            {
                _db.UserCustomerAction.Add(UserCustomerAction);
                _db.SaveChanges();
            }
        //actionId == 2 Latest_videocall 
            if (UserCustomerAction.actionId == 2 && duplicate == false)
            {
                _db.UserCustomerAction.Add(UserCustomerAction);
                _db.SaveChanges();
            }

            //actionId == 2 Latest_videocall 
            if (UserCustomerAction.actionId == 3 && duplicate == false)
            {
                _db.UserCustomerAction.Add(UserCustomerAction);
                _db.SaveChanges();
            }

            //redirect with the customer id to the same page
            var usercustomer = _db.UserCustomer.Find(UserCustomerAction.usercustomerId);
            var DebiteurnrId = usercustomer.DebiteurnrId;

            var redirect = string.Format("edit/{0}/customerType/{1}/klant/{2}", DebiteurnrId, customerType,klant);

            



            return Redirect(redirect);
            
            
        }

        // POST: UserCustomerController/Delete/5
        public ActionResult removecustomer(string id, string CustomerType)
        {    

            var loggedInId = _usermanager.GetUserId(HttpContext.User);
            var customer = _db.UserCustomer.Where(x => x.userid == loggedInId && x.DebiteurnrId == id && x.customerType == CustomerType).First();

            _db.UserCustomer.Remove(customer);
            _db.SaveChanges();

            return Ok();
        }

        public ActionResult notifications()
        {
            return View();
        }
    }
}
