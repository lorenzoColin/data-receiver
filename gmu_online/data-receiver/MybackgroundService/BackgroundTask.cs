using data_receiver.Data;
using data_receiver.ElasticConnection;
using data_receiver.Models;
using data_receiver.Models.ViewModels;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.AspNetCore.Identity;
using data_receiver.google_ads;


namespace data_receiver.MybackgroundService
{
    public class BackgroundTask : BackgroundService
    {

        private readonly ILogger<BackgroundTask>  _logger;
        private readonly IServiceProvider _service;
        private readonly ElasticClient _client;
       



        public BackgroundTask( ILogger<BackgroundTask> logger, IServiceProvider services)
        {
            _client = new ElasticSearchClient().EsClient();
            _service = services;
            _logger = logger;

        }
        //currentBudget is action id 1
        public async Task currentBudget()
        {
            using var scope = _service.CreateScope();
            var   client = new ElasticSearchClient().EsClient();
            var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sendmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
            var GoogleAds = new GoogleAds();



            //count of the month
            int countDaysMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            //result 40 is the value of the user input
            IEnumerable<UserCustomerAction> userCustomerActions = _db.UserCustomerAction;
              
            var UserCustomerActionByDays = new List<UserCustomerActionByDay>();
            foreach (var action in userCustomerActions)
            {
                if(action.actionId == 1)
                {
                    var result = (countDaysMonth / 100.0 * action.value);
                    //add de dag en de usercustomerId
                    UserCustomerActionByDays.Add(new UserCustomerActionByDay { day = Math.Round(result, MidpointRounding.AwayFromZero), usercustomerId = action.usercustomerId });
                
                }
               
            }

            //day of today
            var day = DateTime.Now.Day;

            //hier loopt ie over een lijst met dagen dat zijn ingevuld door de gebruiker
            foreach (var UserCustomerActionByDay in UserCustomerActionByDays)
            {

               

                //find the customer 
                var usercustomer = _db.UserCustomer.Find(UserCustomerActionByDay.usercustomerId);
                var allcustomer = new CustomerList(_db);
                var customers = allcustomer.getallcustomers();
                var singlecustomer = customers.Where(s => s.customer.CustomerType == usercustomer.customerType && s.customer.Debiteurnr == usercustomer.DebiteurnrId).FirstOrDefault();
                var AdsAccountName = singlecustomer.customer.Ads.Trim();
                
                //list with customer and the customer id
                var customerlist = GoogleAds.customerList();
                

                //krijg de klanten naam met customer id            
                var singelist = customerlist.Where(s => s.accountName == AdsAccountName).FirstOrDefault();
                if(singelist != null)
                {
                   var cost = Math.Round( GoogleAds.getcustomerWithCost(singelist.accountId.ToString()).Sum(),2);

                   var count = UserCustomerActionByDay.day / countDaysMonth  * 100 ;
                   var value = Math.Round(count, MidpointRounding.AwayFromZero);


                /* 
                1.search in the usercustomeraction for the specific user with the value
                 */
                   var SingleUserCustomerAction = _db.UserCustomerAction.Where(s => s.usercustomerId == UserCustomerActionByDay.usercustomerId && s.value == value && s.action.id == 1  ).FirstOrDefault();

                    //sla koste op in database
                    if (SingleUserCustomerAction != null)
                    {
                        //if the user hase not cost yet add the cost
                        if (SingleUserCustomerAction.cost == null)
                        {
                            SingleUserCustomerAction.cost = cost;
                            _db.SaveChanges();
                        }
                        //if the user already has a cost update em with the cost of the next day
                        if (SingleUserCustomerAction.cost != null)
                        {
                            SingleUserCustomerAction.cost = cost + SingleUserCustomerAction.cost;
                            _db.SaveChanges();
                        }
                        //if the date of today is equal to when a user wants to receive a mail
                        //send a mail with the cost of the month
                        if(UserCustomerActionByDay.day == day)
                        {
                            //send a mail with cost and max budget
                            Console.WriteLine(String.Format( "jij heb deze maand {0} euro verspild", SingleUserCustomerAction.cost) );


                            //when the mail is send the total cost is going to be zero again
                            SingleUserCustomerAction.cost = null;
                            _db.SaveChanges();

                        }
                    }

                }

            }
        }
        //LastVideoCall is action id 2
        public  async Task LastVideoCall()
        {
            using var scope = _service.CreateScope();
            var client = new ElasticSearchClient().EsClient();
            var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sendmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();


            IEnumerable<UserCustomerAction> userCustomerActions = _db.UserCustomerAction;


            foreach (var usercustomerAction in userCustomerActions)
            {
                //if actionId 2 laaste video call gesprek
                //naam moet actionName worden
                if(usercustomerAction.actionId == 2)
                {
                    var usercustomer = _db.UserCustomer.Find(usercustomerAction.usercustomerId);
                    var allcustomer = new CustomerList(_db);
                    var getallcustomers = allcustomer.getallcustomers();



                    var customer = getallcustomers.Where(s => s.customer.CustomerType == usercustomer.customerType && s.customer.Debiteurnr == usercustomer.DebiteurnrId).First();
                    var user = _db.Users.Find(usercustomer.userid);


                    //als customer niet null is en customer heeft een latest_video call propperty dan kom je in de if statement
                    if (customer != null && customer.customer.Latest_videocall != "Geen behoefte aan bij de klant.")
                    {
                        var Datenow = DateTime.Now.ToString("d/M/yyyy");
                        var DateOftoday =   DateTime.Parse(Datenow);

                        var Latest_videocall = customer.customer.Latest_videocall;
                        var Latest_videocallDate = DateTime.Parse(Latest_videocall);



                        //if the latest call is in the future for example 11-06-2022 last video call
                        if(Latest_videocallDate > DateOftoday)
                        {
                            Latest_videocallDate = Latest_videocallDate.AddYears(-1);
                        }

                        //this function gonne look at the latest_videcaldate and adds the value of the month to it
                        var reminderMailDay = Latest_videocallDate.AddMonths(usercustomerAction.value).ToString("d/M/yyyy");
                        var remindermailtype = DateTime.Parse(reminderMailDay);


                        //if de date of now is equal to the remindermailDate
                        if (Datenow == reminderMailDay)
                        {
                            //send mail to the user 
                            Console.WriteLine(string.Format("reminder {0} plan your next video call in with the company {1} contact is {2} ", user.Email, customer.customer.Klant, customer.customer.Contact));
              
                            //send mail to the customer
                            Console.WriteLine(String.Format("reminder {0} from {1} your last video call is a long time ago  ", customer.customer.Klant, customer.customer.Contact));
                        } 

                    }
                }
            }
            await Task.CompletedTask;
        }

        //deze moet gemaakt worden
        //LastContact is action Id 3
        public async Task LastContact()
        {
            using var scope = _service.CreateScope();
            var client = new ElasticSearchClient().EsClient();
            var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sendmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
            //var httpcontext = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IEnumerable<UserCustomerAction> userCustomerActions = _db.UserCustomerAction;


            foreach (var usercustomerAction in userCustomerActions)
            {
                //if actionId 3 laaste contact call gesprek
                if (usercustomerAction.actionId == 3)
                {
                    var usercustomer = _db.UserCustomer.Find(usercustomerAction.usercustomerId);
                    var allcustomer = new CustomerList(_db);
                    var getallcustomers = allcustomer.getallcustomers();

                    var customer = getallcustomers.Where(s => s.customer.CustomerType == usercustomer.customerType && s.customer.Debiteurnr == usercustomer.DebiteurnrId).First();
                    var user = _db.Users.Find(usercustomer.userid);


                    //dit is de laatste keer dat ze hebben gebeld
                    var Latest_contact = customer.customer.Latest_contact;
                    var Latest_contactDate = DateTime.Parse(Latest_contact);

                    //grab the date from now and add the months when you want to receive an email
                    var reminderMailDay = Latest_contactDate.AddMonths(usercustomerAction.value).ToString("d/M/yyyy");

                    
                    var DateOftoday = DateTime.Now.ToString("d/M/yyyy");
                    var day = DateTime.Now;


                    if (Latest_contactDate > day)
                    {
                        Latest_contactDate = Latest_contactDate.AddYears(-1);
                    }

                    //if de date of now is equal to the remindermailDate
                    if (DateOftoday == reminderMailDay)
                    {
                        Console.WriteLine("sasa");
                        //send mail to user that he has not speak to his contact for a long time 
                        //and how long ago
                    }
                }

            }
            await Task.CompletedTask;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken )
        {
            //zo lang de app niet sluit
                while(!stoppingToken.IsCancellationRequested)
                {
                
                currentBudget();
                //LastContact();
                //LastVideoCall();
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
                }
        }

        //cancellationToken is de token die je krijgt als je app opstart
        //als deze true is betekent dat je app uit staat
        public override Task StopAsync(CancellationToken CancellationToken)
        {
            Console.WriteLine("done");
            return base.StopAsync(CancellationToken);
        } 
    }


}

