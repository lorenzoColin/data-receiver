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
using System.Linq;

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

        var user = _usermanager.GetUserId(HttpContext.User);
        //ingelogde user
        var loggedInUser = await _db.Users.FindAsync(user);


        var usercustomerviewmodel = new List<UserCustomerViewModel>();

        //1.kijken of de ingelogde gebruiker niet in pitvot table staat
        //2.kijken naar klanten die nog niet in pivot table staat
        var usercustomer = _db.UserCustomer;
        foreach (var item in usercustomer)
        {

            //laat alle customer zien van de ingelogde user
            if (item.UserId == user)
            {

                var search = await _client.SearchAsync<Customer>(s => s.Query(s => s.Match(f => f.Field(f => f.Debiteurnr).Query(item.DebiteurnrId))));
                var live_clients = search.Documents.FirstOrDefault(s => s.Debiteurnr == item.DebiteurnrId);
                var applicationUser = _db.Users.Find(item.UserId);

                usercustomerviewmodel.Add(
                        new UserCustomerViewModel
                        {
                            Users = applicationUser,
                            Customer = live_clients,
                            DebiteurnrId = item.DebiteurnrId,
                            userId = item.UserId,
                        });

            }
        }
        return View(usercustomerviewmodel);

        }
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult claimContact(CustomerContactViewModel customercontact)
        //{
        //    var cutomercont = new CustomerContact { contactId = customercontact.contactId, customerId = customercontact.customerId};
        //    _db.CustomerContact.Add(cutomercont);
        //    _db.SaveChanges();
        //    return RedirectToAction("mycontact",customercontact.customerId);
        //}
        //public ActionResult mycontact(Customer customer)
        //{
        //    ViewBag.customerId = customer.Debiteurnr;
        //    //alle contacten die nog niet in die customer zitten
        //    string customerId = customer.Debiteurnr;
        //    string query = @" select c. ,uc.customerId,uc.contactId from [Identity].[Contact] as c 
        //        left outer join  [Identity].[CustomerContact] as uc  on c.id = uc.contactId 
        //        left outer join  [Identity].[customer] as u  on u.id = uc.customerId 
        //        where not exists (SELECT  contactId,customerId 
        //        from [Identity].[CustomerContact] uc 
        //        where  uc.customerId = {0}
        //        and uc.contactId = c.id)";
        //   IEnumerable<Contact> contact = _db.Contact.FromSqlRaw(query, customerId ).Distinct();

        //    return View(contact);
        //}

        
        [Route("usercustomer/edit/{DebiteurnrId}")]
        public async Task<ActionResult> Edit(string DebiteurnrId)
        {
            //getting al result out of the debiteurId
            var result = new List<UserCustomerActionViewModel>();
            var search = await _client.SearchAsync<Customer>(s => s.Query(s => s.Match(f => f.Field(f => f.Debiteurnr).Query(DebiteurnrId))));
            var live_clients = search.Documents.FirstOrDefault();
            var loggedInId = _usermanager.GetUserId(HttpContext.User);
            var usercustomer = _db.UserCustomer.Where(s => s.UserId == loggedInId && s.DebiteurnrId == live_clients.Debiteurnr).FirstOrDefault();
            var usercustomerId = usercustomer.Id;

            IEnumerable<UserCustomerAction> usercustomeraction = _db.UserCustomerAction.Where(s => s.usercustomerId == usercustomerId);

            IEnumerable<action> action = _db.action;
            var liveclients = live_clients;

            result.Add(new UserCustomerActionViewModel { live_clients = liveclients, action = action, usercusotmerId = usercustomerId, UserCustomerAction = usercustomeraction });

            return View(result);

        }
        // POST: UserCustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditPost(UserCustomerAction UserCustomerAction)
        {
            var userCustomerActions = _db.UserCustomerAction.Where<UserCustomerAction>(s => s.usercustomerId == UserCustomerAction.usercustomerId);
            var duplicateUsercustomerAction = new List<UserCustomerAction>();
            bool duplicate = false;

            //loop over alle usercustomeraction die jij heb
            foreach (var userCustomerAction in userCustomerActions)
            {
                
                switch (userCustomerAction.actionId)
                {
                    //if actie id 1(mail per maand)
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
                        case 3:
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
            if (UserCustomerAction.actionId == 1 &&  duplicate == false)
            {
                _db.UserCustomerAction.Add(UserCustomerAction);
                _db.SaveChanges();
            }
            if (UserCustomerAction.actionId == 3 && duplicate == false)
            {
                _db.UserCustomerAction.Add(UserCustomerAction);
                _db.SaveChanges();
            }
            //redirect with the customer id to the same page
            var usercustomer = _db.UserCustomer.Find(UserCustomerAction.usercustomerId);
            var DebiteurnrId = usercustomer.DebiteurnrId;
            var redirect = string.Format("edit/{0}", DebiteurnrId);
            return Redirect(redirect);
        }

        // POST: UserCustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult removecustomer(string DebiteurnrId)
        {    
            var loggedInId = _usermanager.GetUserId(HttpContext.User);
            var usercustomer = new UserCustomer { UserId = loggedInId, DebiteurnrId = DebiteurnrId };
            var customer = _db.UserCustomer.Where(x => x.UserId == loggedInId && x.DebiteurnrId == usercustomer.DebiteurnrId).First();

            _db.UserCustomer.Remove(customer);
            _db.SaveChanges();

            return Ok();
        }
    }
}
