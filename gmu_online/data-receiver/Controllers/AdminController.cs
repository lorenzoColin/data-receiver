using data_receiver.Data;
using data_receiver.Models;
using data_receiver.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace data_receiver.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<ApplicationUser> _usermanager;
        public AdminController(ApplicationDbContext db, UserManager<ApplicationUser> usermanager)
        {
    
            _usermanager = usermanager;
            _db = db;
        }
        // GET: AdminController
        public ActionResult AllUsers()
        {

            IEnumerable<ApplicationUser> users = _db.Users;
            return View(users);
        }


        // GET: AdminController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AdminController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <ActionResult> Create(CreateUserViewModel CreateUserViewModel)
        {
          var user =  new ApplicationUser {Email = CreateUserViewModel.email,FirstName = CreateUserViewModel.FirstName,LastName = CreateUserViewModel.LastName,UserName = CreateUserViewModel.email};

            var result = await _usermanager.CreateAsync(user, CreateUserViewModel.Password);


            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View();
        }

        // GET: AdminController/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("allUsers");
            }
            var user = _db.Users.Find(id);
            return View(user);
        }

        // POST: AdminController/Edit/5
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
        // GET: AdminController/Delete/5
        [HttpGet]
        public ActionResult Delete(string? id)
        {
            if (id == null)
            {
                return RedirectToAction("allUsers");
            }
            var user = _db.Users.Find(id);

            return View(user);
        }

        // POST: AdminController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PostDelete(string? id)
        {
            var user = _db.Users.Find(id);

            if (id == null)
            {
                return View();
            }

            _db.Users.Remove(user);
            _db.SaveChanges();
            return RedirectToAction("allUsers");
        }
    }
}
