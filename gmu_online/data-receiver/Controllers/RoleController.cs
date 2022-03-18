using data_receiver.Models;
using data_receiver.Models.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace data_receiver.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _userroles;
        private readonly UserManager<ApplicationUser> _userManager;
        public RoleController( RoleManager<IdentityRole> userroles, UserManager<ApplicationUser> userManager)
        {
            _userroles = userroles;
            _userManager = userManager;
        }

        // GET: RoleController
        public ActionResult Index()
        {
            IEnumerable<IdentityRole> roles = _userroles.Roles;

            return View(roles);
        }

        // GET: RoleController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: RoleController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RoleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Role Role)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            var test = await _userroles.RoleExistsAsync(Role.RoleName);

            if (test)
            {
                ModelState.AddModelError("role exist", "role already exist");
            }
            await _userroles.CreateAsync(new IdentityRole(Role.RoleName));

            return View();
        }
        // GET: RoleController/Edit/5
        public async Task<ActionResult> Edit(string id)
        { 
           var role = await _userroles.FindByIdAsync(id);

            if (role == null)
            {
                return NotFound();
            }
            // model with the id and name
            var model = new EditRoleViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name
            };
            //get a list of user and loop over them
            foreach (var user in _userManager.Users.ToList())
            {
                //check if the user that i loop over have the same rol
                //check
                if( await _userManager.IsInRoleAsync(user,model.RoleName))
                {
                    model.users.Add(user.UserName.ToString());
                }
            }


            return View(model);
        }

        // POST: RoleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Edit( EditRoleViewModel EditRoleViewModel)
        {
            if (ModelState.IsValid)
            {

            }
            if (ModelState.IsValid)
            {
                try
                {
                    var role = await _userroles.FindByIdAsync(EditRoleViewModel.RoleId);
                    role.Name = EditRoleViewModel.RoleName;
                    var result = await _userroles.UpdateAsync(role);
                    if (!result.Succeeded)
                    {
                        ModelState.AddModelError("", result.Errors.First().ToString());
                        return View();
                    }
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View();
        }

        // GET: RoleController/Delete/5
        public async Task<ActionResult>Delete(string id)
        {
            var role = await _userroles.FindByIdAsync(id);
            
            if(role == null)
            {
                return NotFound();   
            }
            return View(role);
        }
        [HttpGet]
        public async Task<ActionResult> editUserInRole(string id)
        {
            //vind de role van nu
            var role = await _userroles.FindByIdAsync(id);


            ViewBag.userid = await _userroles.FindByIdAsync(id);

            //maak een lijst met userroleModel
            var model = new List<UserRoleViewModel>();

            //loop over alle users
            foreach (var user in _userManager.Users)
            {
                //maak een object en vull deze met de users 
                var userRoleViewModel = new UserRoleViewModel()
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                    UserName = user.UserName,
                };



                //als de user waar we over heen loopen in de roll
                //zit voegen we deze toe aan de list hier boven
                //en zet de zet de propperty van inrole naar true
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    //dan check ik in mijn view op objecten met true en objecten met false
                    model.Add(userRoleViewModel);
                    userRoleViewModel.InRole = true;
                }

                if (!await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Add(userRoleViewModel);
                    userRoleViewModel.InRole = false;
                }


            }
            //dan sturen we deze lijst naar view
            return View(model);

        }

        [HttpPost]
        public async Task<ActionResult> editUserInRole(UserRoleViewModel userRole)
        {
            var user = await _userManager.FindByIdAsync(userRole.UserId);
            var role = await _userroles.FindByIdAsync(userRole.RoleId);

            await _userManager.AddToRoleAsync(user, role.Name) ;
            return View(userRole);
            

        }
     
        
      
        // POST: RoleController/Delete/5
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
