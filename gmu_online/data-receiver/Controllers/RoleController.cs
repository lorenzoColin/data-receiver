using data_receiver.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace data_receiver.Controllers
{
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _userroles;
        public RoleController( RoleManager<IdentityRole> userroles)
        {
            _userroles = userroles;
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
            
            var test = await  _userroles.RoleExistsAsync(Role.RoleName); 

            if (test)
            {
                ModelState.AddModelError("role exist","role already exist");
            }
             await _userroles.CreateAsync(new IdentityRole(Role.RoleName));         

            return View();
        }

        // GET: RoleController/Edit/5
        public async Task<ActionResult> Edit(string id)
        {
           var singleRole = await _userroles.FindByIdAsync(id);
            return View(singleRole);
        }

        // POST: RoleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task< ActionResult> Edit( IdentityRole IdentityRole)
        {
            if (ModelState.IsValid)
            {
                try
                {   
                    var role = await _userroles.FindByIdAsync(IdentityRole.Id);
                    role.Name = IdentityRole.Name;
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
