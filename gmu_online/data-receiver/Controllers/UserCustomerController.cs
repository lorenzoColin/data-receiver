using data_receiver.Data;
using data_receiver.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            return View();
        }

        // POST: UserCustomerController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
