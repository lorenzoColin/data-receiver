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
        public async Task< ActionResult> Edit(string DebiteurnrId)
        {
         

            var search = await _client.SearchAsync<Customer>(s => s.Query(s => s.Match(f => f.Field(f => f.Debiteurnr).Query(DebiteurnrId))));

            var live_clients = search.Documents.FirstOrDefault();

            var loggedInId = _usermanager.GetUserId(HttpContext.User);

            var test  = _db.UserCustomer.Where(s => s.UserId == loggedInId && s.DebiteurnrId == live_clients.Debiteurnr).FirstOrDefault();

            if(test != null)
            {
                ViewBag.usercustomer = test.Id;
            }

            //  //action a user can choose
            //    string queryAction = @"select * from [Identity].[action] as a
            //    where not exists(select * from [Identity].[Customer] where actionId = a.id and id = {0} )";
            //    ViewBag.actionlist = _db.action.FromSqlRaw(queryAction,id).Select(s => new SelectListItem
            //    {
            //        Text = s.description,
            //        Value = s.id.ToString()
            //    }).ToList<SelectListItem>();

            //    //current action
            //    string currentActionstring = @"select a.id, a.Name,c.ActionId from [Identity].[action] as a
            //                            inner join [Identity].[Customer] as c on a.id = c.actionId
            //                            where c.Id = {0}";

            //   ViewBag.currentAction = _db.action.FromSqlRaw(currentActionstring, id).ToList();

            ViewBag.action = _db.action;

            return View(live_clients);
            }
        // POST: UserCustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult edit(UserCustomerAction UserCustomerAction)
        {
            var mine = _db.UserCustomerAction.Where<UserCustomerAction>(s => s.usercustomerId == UserCustomerAction.usercustomerId);



            var list = new List<UserCustomerAction>();

            //loop over alle usercustomeraction die jij heb
            foreach (var sej in mine)
            {
                if(sej.value == UserCustomerAction.value)
                {

                  var sejjjjj =   _db.UserCustomerAction.Find(sej.id);

                    list.Add(sejjjjj);
                    break;
                }
               
            }

            list[0].value = UserCustomerAction.value;
            _db.SaveChanges();


            
            return RedirectToAction("edit");
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
