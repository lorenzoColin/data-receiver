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
            //my user id 
            var user = _userManager.GetUserId(HttpContext.User);
            //ingelogde user
            var loggedInUser = await _db.Users.FindAsync(user);

            //sma klanten
            var search = _client.Search<Sma_klanten>(s => s.Index("sma_klanten"));
            var sma_klanten = search.Documents;
            var CustomerViewModel = new List<CustomerViewModel>();

            

            //dit zijn alle sma klanten
            foreach (var sma in sma_klanten)
            {                
                CustomerViewModel.Add(new CustomerViewModel
                {
                    customerType = "sma_klanten",
                    customer = new Customer
                    {
                        Debiteurnr = sma.Debiteurnr,
                        Klant = sma.Klant,
                        Consultant = sma.Beheerder,
                        Datum_live = sma.datum_live,
                        Max_budget = sma.budget_afspr,
                        Doelstelling = sma.Doelstelling,
                        Servicefee_afspraak = sma.Servicefee_afspraak,
                        CustomerType = "sma_klanten"
                    }
                });
            }


            var search1 = _client.Search<Customer>(s => s.Index("sea_klanten"));
            var sea_klanten = search1.Documents;
            foreach (var sea in sea_klanten)
            {
                CustomerViewModel.Add(new CustomerViewModel
                {
                    customerType = "sea_klanten",
                    customer = sea,
                });

                sea.CustomerType = "sea_klanten";
            }



            //dit zijn alle klanten van sma en sea
            var AllCustomers = CustomerViewModel;



            //my customers
            IEnumerable<UserCustomer> userCustomers = _db.UserCustomer;
            //list to add my customers
            var mycustomers = new List<CustomerViewModel>();

            foreach (var mycustomer in userCustomers)
            {
                //mijn user ophalen vanuit die table
                if (mycustomer.userid == user)
                {
                    foreach(var test in AllCustomers)
                    {
                        if(mycustomer.DebiteurnrId == test.customer.Debiteurnr && mycustomer.customerType == test.customer.CustomerType)
                        {
                            mycustomers.Add(new CustomerViewModel { customer = test.customer,customerType = test.customerType } );
                        }
                    }
                    
                    //var searchclient = await _client.SearchAsync<Customer>(s => s.Query(s => s.Match(f => f.Field(f => f.Debiteurnr).Query(mycustomer.DebiteurnrId))));
                    //var singlecustomer = searchclient.Documents.FirstOrDefault(s => s.Debiteurnr == mycustomer.DebiteurnrId);
                    //mycustomers.Add(singlecustomer.Debiteurnr, singlecustomer.Contact);
                }
            }



         
           
            ////hier pakt ik alle customer en haal ik mijn customer eraf
            //var customers = AllCustomers.Except(mycustomers).ToArray();





            //var sma_klanten = _client.Search<Sma_klanten>(s => s.Index("sma_klanten"));
            ////my user id 
            //var user = _userManager.GetUserId(HttpContext.User);
            ////ingelogde user
            //var loggedInUser = await _db.Users.FindAsync(user);

            ////elastic search client
            //var search = await _client.SearchAsync<Customer>();
            //var live_clients = search.Documents;

            ////my customers
            //IEnumerable<UserCustomer> userCustomers = _db.UserCustomer;

            ////list to add all customers
            //var allcustomers = new Dictionary<string, string>();

            ////list to add my customers
            //var mycustomers = new Dictionary<string, string>();

            //foreach (var mycustomer in userCustomers)
            //{
            //    if (mycustomer.UserId == user)
            //    {
            //        var searchclient = await _client.SearchAsync<Customer>(s => s.Query(s => s.Match(f => f.Field(f => f.Debiteurnr).Query(mycustomer.DebiteurnrId))));
            //        var singlecustomer = searchclient.Documents.FirstOrDefault(s => s.Debiteurnr == mycustomer.DebiteurnrId);
            //        mycustomers.Add(singlecustomer.Debiteurnr, singlecustomer.Contact);
            //    }
            //}
            //foreach (var live_client in live_clients)
            //{
            //    allcustomers.Add(live_client.Debiteurnr, live_client.Contact);
            //}

            ////hier haal de klanten die de ingelogde gebruiker heeft weg van de lijst met alle klanten
            ////nu hou ik klanten over die de ingelogde gebruiker niet beheerd
            //var customers = allcustomers.Except(mycustomers).ToArray();

            //var customertoclaim = new List<Customer>();

            //foreach (var customer in customers)
            //{
            //    var searchclient = await _client.SearchAsync<Customer>(s => s.Query(s => s.Match(f => f.Field(f => f.Debiteurnr).Query(customer.Key))));
            //    var result = searchclient.Documents.FirstOrDefault();

            //    customertoclaim.Add(result);
            //}
            //return View(customertoclaim);

            return View();
        }
        // GET: CustomerController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Customer Customer)
        {
            if (ModelState.IsValid)
            {
                //_db.cu.Add(Customer);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                return BadRequest();
            }
        }
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



            
            

            //var customer =  _db.Customer.Find(id);

            
            return View();
        }

  

        // GET: CustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            //var customer = _db.Customer.Find(id);
            return View();
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            //var customer = _db.Customer.Find(id);

            //if (customer == null)
            //{
            //    return NotFound();
            //}

           
            //_db.Customer.Remove(customer);
            _db.SaveChanges();
            return RedirectToAction("index");
        }
        public async Task<ActionResult> claimCustomer(string id)
        {
            


            //my user id 
            var userid = _userManager.GetUserId(HttpContext.User);
            //ingelogde user
            var loggedInUser = await _db.Users.FindAsync(userid);

            //var userCustomer = new UserCustomer { DebiteurnrId = id, UserId = loggedInUser, UserId = userid };

            //_db.UserCustomer.Add(userCustomer);
            //_db.SaveChanges();


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
