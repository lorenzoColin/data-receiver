using data_receiver.Data;
using data_receiver.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace data_receiver.Controllers
{
    public class UserCustomerController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _usermanager;
        public UserCustomerController(ApplicationDbContext db, UserManager<ApplicationUser> usermanager)
        {
            _usermanager = usermanager;
            _db = db;
        }
        // GET: UserCustomerController
        public ActionResult Index()
        {   
            var loggedInId = _usermanager.GetUserId(HttpContext.User);
            var mycustomers = _db.UserCustomer.Include(u => u.User).Include(s => s.customer).Where(s => s.User.Id == loggedInId);
            return View(mycustomers);
        }

        // GET: UserCustomerController/Details/5
        public ActionResult Details(int id)
        {
            
            return View();
        }
        // GET: UserCustomerController/Create

        public ActionResult Create()
        {
            //hier maak ik een instantie van die viewmodel
            var vm = new UserCustomerViewModel();

            vm.customers = _db.Customer
                .Where(s => !_db.UserCustomer.Select(s => s.customerId).Contains(s.Id))
                .Select(s => new SelectListItem { Value = s.Id.ToString(), Text = s.firstname })
                .ToList();

            vm.users = _db.Users.Select(s => new SelectListItem {Value = s.Id , Text = s.FirstName }).ToList();

            return View(vm);
        }

        // POST: UserCustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserCustomerViewModel UserCustomerViewModel)
        {

            if (ModelState.IsValid)
            {
                var customer = new UserCustomer { customerId = UserCustomerViewModel.customerId, UserId = UserCustomerViewModel.userId };
                

                _db.UserCustomer.Add(customer);
                _db.SaveChanges(); 
                return Ok();
            }
            return BadRequest();
            
        }

        // GET: UserCustomerController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserCustomerController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
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

        // GET: UserCustomerController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserCustomerController/Delete/5
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
