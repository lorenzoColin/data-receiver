using data_receiver.Data;
using data_receiver.Models;
using data_receiver.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            var UserRoleViewModels = new List<UserRoleViewModel>();  

            

            var users = _db.Users.ToList();
            var roles = _db.Roles.ToList();
            var UserRoles = _db.UserRoles.ToList();
            var USerCustomers = _db.UserCustomer.ToList();



            foreach(var UserRoleViewModel in UserRoleViewModels)
            {
                foreach(var role in roles)
                {
                    if(await _usermanager.IsInRoleAsync(UserRoleViewModel.User, role.Name))
                        {
                            UserRoleViewModel.RoleId = role.Id;
                            UserRoleViewModel.RoleName = role.Name; 
                        }

                }
            }

            foreach (var user in users)
            {
                
            }


            return View(UserRoleViewModels);
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

         var actionresult =    _db.action.Where(s => s.description == ActionName).ToList();

         var action = new Action() { description = ActionName };
            

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
            var UserRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();
            var MyRoles = new List<string>();
            var unclaimedRoles = new List<string>();
            var selectlistItems = new List<SelectListItem>();
        

        foreach(var role in roles)
        {
                //als de user niet in een bepaalde role zit voeg ze toe aan deze lijst
                if ( !await _usermanager.IsInRoleAsync(user, role.Name))
                {
                    selectlistItems.Add(new SelectListItem {Text = role.Name,Value = role.Id,Disabled = false });
               
                }

                //my roles
                if (await _usermanager.IsInRoleAsync(user, role.Name))
                {
                    MyRoles.Add(role.Name);
                }

            }

       


        ViewBag.listItems = selectlistItems;
        ViewBag.MyRole = MyRoles;

          



            return View(user);
        }
        // POST: AdminController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Edit(ApplicationUser Users)
        {

            try
            {
              var User =  await _db.Users.FindAsync(Users.Id);

                User.FirstName = Users.FirstName;
                User.LastName = Users.LastName;
                User.PhoneNumber = Users.PhoneNumber;
                User.Email = Users.Email;
                User.UserName = Users.UserName;
               
                _db.SaveChanges();

                return RedirectToAction("AllUsers");
            }catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

        }

        public async Task< ActionResult> setRole(string RoleId,string userId)
        {


            var user = _db.Users.Find(userId);
            var role = _db.Roles.Find(RoleId);

            if(role == null || user == null)
            {
                return NotFound();
            }
           
            await _usermanager.AddToRoleAsync(user, role.Name);
            
          



            return RedirectToAction("Edit",user.Id);

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
