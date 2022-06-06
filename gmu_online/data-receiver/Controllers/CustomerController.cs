using data_receiver.Data;
using data_receiver.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using data_receiver.Models.ViewModels;
using Nest;
using data_receiver.ElasticConnection;
using System.Collections.Generic;

namespace data_receiver.Controllers
{
    
    public class CustomerController : Controller
    {

        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ElasticClient _client;

        // GET: CustomerController
        public CustomerController(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            _client = new ElasticSearchClient().EsClient();
            _userManager = userManager;
            _db = db;
        }

        public async Task<ActionResult> Index() 
        {

            var user = _userManager.GetUserId(HttpContext.User);
            var loggedInUser = _db.Users.Find(user);

            //customer that hase no relationship with the current users
            var customerList = new CustomerList(_db);
            var unclaimedCustomerlist =  customerList.unclaimedCustomerlist(user);

            return View(unclaimedCustomerlist);
        }
        // GET: CustomerController/Create
     

        // GET: CustomerController/Edit/5
        public ActionResult Edit(int id)
        {
            //action a user can choose
            string queryAction = @"select * from [Identity].[action] as a
            where not exists(select * from [Identity].[Customer] where actionId = a.id and id = {0} )";
            ViewBag.actionlist = _db.action.FromSqlRaw(queryAction,id).Select(s => new SelectListItem
            {
                Text = s.description,
                Value = s.id.ToString()
            }).ToList<SelectListItem>();
            //current action
            string currentActionstring = @"select a.id, a.Name,c.ActionId from [Identity].[action] as a
                                    inner join [Identity].[Customer] as c on a.id = c.actionId
                                    where c.Id = {0}";
            ViewBag.currentAction = _db.action.FromSqlRaw(currentActionstring, id).ToList();            

            return View();
        }


        // GET: CustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            _db.SaveChanges();
            return RedirectToAction("index");
        }
        public async Task<ActionResult> claimCustomer(string id, string customerType )
        {
            var userid = _userManager.GetUserId(HttpContext.User);
            var loggedInUser = await _db.Users.FindAsync(userid);

            //make a new usercustomer object and save this in the database
            var userCustomer = new UserCustomer { DebiteurnrId = id, userid = userid, customerType = customerType };

            _db.UserCustomer.Add(userCustomer);
            _db.SaveChanges();

            return RedirectToAction("index");
        }
        

        public ActionResult setTrigger(int id, int triggerId)
        {
        //var customer = _db.Customer.Find(id);

            //customer.actionId = triggerId;

            //_db.Update(customer);
            _db.SaveChanges();

            return View();

    }
}
}
