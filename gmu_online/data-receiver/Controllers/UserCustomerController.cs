using data_receiver.Data;
using data_receiver.Models;
using data_receiver.Models.ViewModels;
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult claimContact(CustomerContactViewModel customercontact)
        {


            var cutomercont = new CustomerContact { contactId = customercontact.contactId, customerId = customercontact.customerId};
            _db.CustomerContact.Add(cutomercont);
            _db.SaveChanges();

            return RedirectToAction("mycontact");
        }

        public ActionResult mycontact(Customer customer)
        {

            ViewBag.customerId = customer.Id;

            //alle contacten die nog niet in die customer zitten
            int customerId = customer.Id;
            string query = @" select c.Id,c.email,c.firstname,c.lastname,c.phonenumber,c.adress, c.city, c.birthdate,uc.customerId,uc.contactId from [Identity].[Contact] as c 
                left outer join  [Identity].[CustomerContact] as uc  on c.id = uc.contactId 
                left outer join  [Identity].[customer] as u  on u.id = uc.customerId 
                where not exists (SELECT  contactId,customerId 
                from [Identity].[CustomerContact] uc 
                where  uc.customerId = {0}
                and uc.contactId = c.id)";
           IEnumerable<Contact> contact = _db.Contact.FromSqlRaw(query, customerId ).Distinct();

           IEnumerable<CustomerContact> customercontact = _db.CustomerContact.Include(s => s.contact).Include( s => s.customer).Where( s => s.customerId == customerId);

            ViewBag.customercontact = customercontact;
            return View(contact);
        }


        // GET: UserCustomerController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        

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


        // POST: UserCustomerController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult removecustomer(int customerId)
        {
            
            try
            {
                var loggedInId = _usermanager.GetUserId(HttpContext.User);


                var usercustomer = new UserCustomer { UserId = loggedInId, customerId = customerId };

                _db.UserCustomer.Remove(usercustomer);
                _db.SaveChanges();


                return RedirectToAction("index");
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
