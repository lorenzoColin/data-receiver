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

namespace data_receiver.Controllers
{
    public class UserCustomerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly ElasticClient _client;
        public UserCustomerController(ApplicationDbContext db, UserManager<ApplicationUser> usermanager)
        {
            _client = new ElasticSearchClient().EsClient();
            _usermanager = usermanager;
            _db = db;
        }
        // GET: UserCustomerController 
        public async Task< ActionResult >Index()
        {
           
        var userId = _usermanager.GetUserId(HttpContext.User);
            //ingelogde user
        var loggedInUser = await _db.Users.FindAsync(userId);

        var customerlist = new CustomerList(_db);

        var claimedcustomers =  customerlist.claimedcustomerlist(userId);
            ViewBag.Actions = _db.action;





            return View(claimedcustomers);
        }
      

        [Route("usercustomer/edit/{DebiteurnrId}/customerType/{customerType}")]
        public async Task<ActionResult> Edit(string DebiteurnrId,string customerType)
        {

        var userId = _usermanager.GetUserId(HttpContext.User);


         //ingelogde user
        var loggedInUser = await _db.Users.FindAsync(userId);
        var customerlist = new CustomerList(_db);
        var mycustomers = customerlist.claimedcustomerlist(userId);


        var Customer =  mycustomers.Where(s => s.customer.CustomerType == customerType && s.customer.Debiteurnr == DebiteurnrId).FirstOrDefault();
        int UserCustomerId = _db.UserCustomer.Where(s => s.userid == userId && s.DebiteurnrId == DebiteurnrId && s.customerType == customerType).First().Id;

        IEnumerable< UserCustomerAction> usercustomeraction = _db.UserCustomerAction.Where(s => s.usercustomerId == UserCustomerId);

        var Action = _db.action;
        var UserCustomerActionViewModel = new UserCustomerActionViewModel { customer = Customer.customer ,action= Action,usercustomerId = UserCustomerId,UserCustomerAction = usercustomeraction };



            return View(UserCustomerActionViewModel);

        }
        
        public ActionResult SetTriggerPopupModel(UserCustomerAction UserCustomerAction, string customerType ,string DebiteurnrId)
        {
            var userId = _usermanager.GetUserId(HttpContext.User);

            var usercustomer = _db.UserCustomer.Where(s => s.customerType == customerType && s.userid == userId && s.DebiteurnrId == DebiteurnrId).FirstOrDefault();
            usercustomer.Id = usercustomer.Id;


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
                            break;
                        }
                        if (duplicate == true)
                        {
                            duplicateUsercustomerAction[0].value = UserCustomerAction.value;
                            _db.SaveChanges();
                            ModelState.AddModelError("duplicate", "duplicate value");
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
        public ActionResult EditPost(UserCustomerAction UserCustomerAction,string customerType)
        {       


            var userCustomerActions = _db.UserCustomerAction.Where<UserCustomerAction>(s => s.usercustomerId == UserCustomerAction.usercustomerId);
            var duplicateUsercustomerAction = new List<UserCustomerAction>();
            bool duplicate = false;
            //loop over alle usercustomeraction die jij heb
            foreach (var userCustomerAction in userCustomerActions)
            {   
                switch (userCustomerAction.actionId)
                {
                    //if actie id 1()
                    case 1  :
                        //if the value already existed
                        if (userCustomerAction.value == UserCustomerAction.value)
                        {
                            var sejjjjj = _db.UserCustomerAction.Find(userCustomerAction.id);
                            duplicateUsercustomerAction.Add(sejjjjj);
                            duplicate = true;
                            break;
                        }
                        if (duplicate ==  true)
                        {
                            duplicateUsercustomerAction[0].value = UserCustomerAction.value;
                            _db.SaveChanges();
                            ModelState.AddModelError("duplicate", "duplicate value");
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
            if (UserCustomerAction.actionId == 1 &&  duplicate == false)
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

            //redirect with the customer id to the same page
            var usercustomer = _db.UserCustomer.Find(UserCustomerAction.usercustomerId);
            var DebiteurnrId = usercustomer.DebiteurnrId;
            var redirect = string.Format("edit/{0}/customerType/{1}", DebiteurnrId,customerType);
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
