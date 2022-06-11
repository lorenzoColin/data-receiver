using data_receiver.Data;
using data_receiver.Models;
using data_receiver.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Action = data_receiver.Models.action;

namespace data_receiver.Controllers
{
    //[Authorize(Roles =  "admin")]
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
        public async Task<ActionResult> AllUsers()
        {
            var UserRoleViewModels = new List<UserRoleViewModel>();
            var users = _db.Users.ToList();
            var roles = _db.Roles.ToList();
            var UserRoles = _db.UserRoles.ToList();
            var USerCustomers = _db.UserCustomer.ToList();

            foreach (var role in roles)
            {
                foreach (var user in users)
                {

                    if (await _usermanager.IsInRoleAsync(user, role.Name))
                    {
                        UserRoleViewModels.Add(new UserRoleViewModel { User = user,RoleName = role.Name });
                    }

                    if (!await _usermanager.IsInRoleAsync(user, role.Name))
                    {
                        UserRoleViewModels.Add(new UserRoleViewModel { User = user });
                    }
                }

            }
               
        
            return View(UserRoleViewModels);
        }





        public ActionResult usercustomer()
        {
            var list = new CustomerList(_db);
            var usercustomers = _db.UserCustomer.ToList();
            var users = _db.Users.ToList();

            var AdminUserCustomerViewModel = new List<AdminUserCustomerViewModel>();

            //var usercustomerlist = list.claimedcustomerlist(usercustomer.userid);

            foreach (var usercustomer in usercustomers)
            {
                foreach(var user in users)
                {
                    if(user.Id == usercustomer.userid)
                    {
                       var mycustomers = list.claimedcustomerlist(usercustomer.userid);
                    }
                }

            }

            return View(AdminUserCustomerViewModel);

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
            return RedirectToAction("allusers");
        }
        // GET: AdminController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {  
            var selectlistItems = new List<SelectListItem>();
            var UserRoleViewModels = new List<UserRoleViewModel>();
            var users = _db.Users.ToList();
            var roles = _db.Roles.ToList();

            foreach (var role in roles)
            {
                foreach (var user in users)
                {
                    if (user.Id == id)
                    {
                        if (await _usermanager.IsInRoleAsync(user, role.Name))
                        {
                            UserRoleViewModels.Add(new UserRoleViewModel { User = user, RoleName = role.Name, InRole = true, RoleId = role.Id });
                        }
                        if (!await _usermanager.IsInRoleAsync(user, role.Name))
                        {
                            selectlistItems.Add(new SelectListItem { Text = role.Name.ToString(), Value = role.Id, Disabled = false });

                            UserRoleViewModels.Add(new UserRoleViewModel { User = user,InRole = false,list = selectlistItems  });
                        }
                    }
                }
            }
            return View(UserRoleViewModels);
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
            return RedirectToAction("Edit", "admin",new {id = userId});
        }
        public ActionResult DeleteRole(string UserId,string RoleId)
        {
            var UserRole =  _db.UserRoles.Where(s => s.UserId == UserId && s.RoleId == RoleId).FirstOrDefault();

            _db.UserRoles.Remove(UserRole);
            _db.SaveChanges();

            return RedirectToAction("Edit", "admin", new { id = UserId });
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
  
    }
}
