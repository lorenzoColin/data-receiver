using data_receiver.Data;
using data_receiver.Models;
using data_receiver.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Action = data_receiver.Models.action;

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
        public async void test(ApplicationUser user)
        {
             var role = await _usermanager.GetRolesAsync(user);
        }
        // GET: AdminController
        public async Task<ActionResult> AllUsers()
        {
            var id = await _db.Users.FindAsync("e34a3af1-4105-4cb3-b84c-5d7f4dd5527a");
            var users = _db.Users.ToList();
            return View(users);
        }

        [HttpGet]
        public ActionResult CreateAction()
        {

            return View();
        }

        //dit is voor mij als developer dit gaat er uit binnekort
        [HttpPost]
        public ActionResult CreateAction(string ActionName)
        {

         var actionresult =    _db.action.Where(s => s.name == ActionName).ToList();

         var action = new Action() { name = ActionName };

         _db.action.Add(action);
         _db.SaveChanges();
         return View();
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
        public async Task<ActionResult> Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction("allUsers");
            }
            var user = await _usermanager.FindByIdAsync(id);
            return View(user);
        }
        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Edit(ApplicationUser Users)
        {
            try
            {
              var bla =  await _db.Users.FindAsync(Users.Id);

                bla.FirstName = Users.FirstName;
                bla.LastName = Users.LastName;
                bla.PhoneNumber = Users.PhoneNumber;
                bla.Email = Users.Email;
                bla.UserName = Users.UserName;
               
                _db.SaveChanges();

                return RedirectToAction("AllUsers");
            }catch (Exception ex)
            {
                return NotFound(ex.Message);
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

            if (user == null)
            {
                return View();
            }
            _db.Users.Remove(user);
            _db.SaveChanges();
            return RedirectToAction("allUsers");
        }
       // [HttpGet]
       //public ActionResult CreateContact()
       // {
       //     return View();
       // }
       // [HttpPost]
       // [ValidateAntiForgeryToken]
       // public ActionResult CreateContact(Contact contact)
       // {
       //     if (ModelState.IsValid)
       //     { 
       //         _db.Contact.Add(contact);
       //         _db.SaveChanges();
       //         return Ok();
       //     }
       //     return NotFound();
       // }
    }
}
