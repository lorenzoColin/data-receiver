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
            //list with customers who have no user
            IEnumerable<customer> customer = _db.Customer.Where(s => !_db.UserCustomer.Select(s => s.customerId).Contains(s.Id));


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
