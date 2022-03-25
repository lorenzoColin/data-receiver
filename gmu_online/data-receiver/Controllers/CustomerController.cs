using data_receiver.Data;
using data_receiver.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace data_receiver.Controllers
{

    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _usermanager;
        // GET: CustomerController

        public CustomerController(ApplicationDbContext db, UserManager<ApplicationUser> usermanager)
        {

            _db = db;
            _usermanager = usermanager;
        }
        [Authorize]
        public ActionResult Index()
        {
            var user = _usermanager.GetUserId(HttpContext.User);

            string query = @"select c.Id,c.firstname,c.lastname,c.phonenumber,c.company,c.adress,c.city,c.actionid,c.birthdate ,uc.userId,customerId from [Identity].[Customer] as c 
            left outer join  [Identity].[UserCustomer] as uc  on c.id = uc.customerId 
            left outer join  [Identity].[User] as u  on u.id = uc.userId where 
            not exists (select userId,customerId 
            from [Identity].[UserCustomer] uc 
            where uc.userId = {0} 
            and uc.customerId = c.id)";

            var customer = _db.Customer.FromSqlRaw(query,user).ToList();

            return View(customer);

        }
        
        public ActionResult claimCustomer(int id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var singleCustomer = _db.Customer.Find(id);

            return View(singleCustomer);
        }

        [HttpPost]
        public ActionResult claimCustomer(customer customer)
        {
            var loggedInUser = _usermanager.GetUserId(HttpContext.User);

            var usercustomer = new UserCustomer { customerId = customer.Id,UserId = loggedInUser };

            _db.UserCustomer.Add(usercustomer);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }
        // GET: CustomerController/Details/5
        public ActionResult Details(int id)
        {
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
        public ActionResult Create(customer customer)
        {    
            if (ModelState.IsValid)
            {
                _db.Customer.Add(customer);
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

            if (id == null)
            {
                return NotFound();
            }
            var singleCustomer = _db.Customer.Find(id);
            return View(singleCustomer);
        }

        // POST: CustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, customer customer)
        {
            if (!ModelState.IsValid)
            {
                _db.Customer.Update(customer);
                _db.SaveChanges();
                return RedirectToAction("index");
            }
            return View(customer);
        }

        // GET: CustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
