using data_receiver.Data;
using data_receiver.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using data_receiver.Models.ViewModels;

namespace data_receiver.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _userManager;


        // GET: CustomerController
        public CustomerController(ApplicationDbContext dbcontext, UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
            _db = dbcontext;
        }
        public ActionResult Index()
        {


            var user = _userManager.GetUserId(HttpContext.User);
            string query = @" select c.Id,c.company,c.phonenumber,c.city,c.adress,c.admin, c.actionId, uc.customerId,uc.UserId from [Identity].[Customer] as c 
                left outer join  [Identity].[UserCustomer] as uc  on c.id = uc.customerId 
                left outer join  [Identity].[User] as u  on u.id = uc.userId 
                where not exists (SELECT  userId,customerId 
                from [Identity].[UserCustomer] uc 
                where  uc.userId = {0}
                and uc.customerId = c.id)";

            IEnumerable<Customer> Customer = _db.Customer.FromSqlRaw(query, user).Distinct();



            var customerView = new List<CustomerActionViewModel>();

            foreach (var sii in Customer)
            {
                var action = _db.action.FirstOrDefault(s => s.id == sii.actionId);
                if(action == null)
                {
                    customerView.Add(new CustomerActionViewModel { customer = sii, CustomerId = sii.Id, });
                    Console.WriteLine("null value");
                }
                if(action!= null)
                {
                    customerView.Add(new CustomerActionViewModel { customer = sii, CustomerId = sii.Id,actionName = action.name,actionId = action.id });
                    var name =  action.name;
                }

            }


            return View(customerView);
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
                _db.Customer.Add(Customer);
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
                Text = s.name,
                Value = s.id.ToString()
            }).ToList<SelectListItem>();

            //current action
            string currentActionstring = @"select a.id, a.Name,c.ActionId from [Identity].[action] as a
                                    inner join [Identity].[Customer] as c on a.id = c.actionId
                                    where c.Id = {0}";

           ViewBag.currentAction = _db.action.FromSqlRaw(currentActionstring, id).ToList();



            
            

            var customer =  _db.Customer.Find(id);

            
            return View(customer);
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public  ActionResult Edit(Customer Customer)
        {



            if (ModelState.IsValid)
            {
                Customer.actionId = Customer.actionId;
                _db.Customer.Update(Customer);
                _db.SaveChanges();
            }
            return RedirectToAction("index");
        }

        // GET: CustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            var customer = _db.Customer.Find(id);
            return View(customer);
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeletePost(int id)
        {
            var customer = _db.Customer.Find(id);

            if (customer == null)
            {
                return NotFound();
            }

           
            _db.Customer.Remove(customer);
            _db.SaveChanges();
            return RedirectToAction("index");
        }
        public ActionResult claimCustomer(int id)
        {
          
            var singleCustomer = _db.Customer.Find(id);

            if (singleCustomer == null)
            {
                return NotFound();
            }

            return View(singleCustomer);
        }
        [HttpPost]
        public async Task<ActionResult> claimCustomer(Customer Customer)
        {
            var loggedInUserId = _userManager.GetUserId(HttpContext.User);

        

            var UserCustomer = new UserCustomer { UserId = loggedInUserId, customerId = Customer.Id };

           await _db.UserCustomer.AddAsync(UserCustomer);
 
           await _db.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public ActionResult setTrigger(int id, int triggerId)
        {
        var customer = _db.Customer.Find(id);

            customer.actionId = triggerId;

            _db.Update(customer);
            _db.SaveChanges();

            return View();

    }
}
}
