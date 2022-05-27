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
            //var httpcontext = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            //var  userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var GoogleAds = new GoogleAds();



            //count of the month
            int countDaysMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            //result 40 is the value of the user input
            IEnumerable<UserCustomerAction> userCustomerActions = _db.UserCustomerAction;
              
            var UserCustomerActionByDay = new List<UserCustomerActionByDay>();
            foreach (var action in userCustomerActions)
            {
                if(action.actionId == 1)
                {
                    var result = (countDaysMonth / 100.0 * action.value);
                    //add de dag en de usercustomerId
                    UserCustomerActionByDay.Add(new UserCustomerActionByDay { day = Math.Round(result, MidpointRounding.AwayFromZero), usercustomerId = action.usercustomerId });
                }
               
            }

            //day of today
            var day = DateTime.Now.Day;


            //hier loopt ie over een lijst met dagen dat zijn ingevuld door de gebruiker
            foreach (var UserCustomerAction in UserCustomerActionByDay)
            {

                var usercustomer = _db.UserCustomer.Find(UserCustomerAction.usercustomerId);
                var allcustomer = new CustomerList(_db);
                var customers = allcustomer.getallcustomers();

                var singlecustomer = customers.Where(s => s.customer.CustomerType == usercustomer.customerType && s.customer.Debiteurnr == usercustomer.DebiteurnrId).First();
                var user = _db.Users.Find(usercustomer.userid);

                //lijst met klanten namen en de customer id van google ads
                var çustomerlist = GoogleAds.customerList();

                //kijken of de current klanten naam in die lijst voorkomt
                //marketing moet alleen de naam nog veranderen
                //zodat ik contains naar is gelijk aan kan veranderen 
                var singelist = çustomerlist.Where(s => s.accountName.Contains(singlecustomer.customer.Klant)).FirstOrDefault();

                if(singelist != null)
                {
                    var cost = GoogleAds.getcustomerWithCost(singelist.accountId.ToString()).Sum();
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
            //var httpcontext = scope.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            //var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

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
                        //date of today
                        var Datenow = DateTime.Now.ToString("d/M/yyyy");
                        //date of now with datime type;
                        var DateOftoday =   DateTime.Parse(Datenow);

                        //dit is de laatste keer dat ze hebben gebeld
                        var Latest_videocall = customer.customer.Latest_videocall;
                        var Latest_videocallDate = DateTime.Parse(Latest_videocall);

                        //this function gonne look at the latest_videcaldate and adds the value of the month to it
                        //het probleem met dit is als je laatste video call 2021 maart is en je vult 3 manden in krijg je em nooit binnen
                        //oplossing voor vinden...
                        var reminderMailDay = Latest_videocallDate.AddMonths(usercustomerAction.value).ToString("d/M/yyyy");
                        var remindermailtype = DateTime.Parse(reminderMailDay);

                        Console.WriteLine(reminderMailDay);
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

                    //date of today
                    var DateOftoday = DateTime.Now.ToString("d/M/yyyy");
                    var day = DateTime.Now;
                  
                    //if de date of now is equal to the remindermailDate
                    if (DateOftoday == reminderMailDay)
                    {
                        //send mail to the user 
                        Console.WriteLine(string.Format("reminder {0} plan your next video call in with the company {1} contact is {2} ", user.Email, customer.customer.Klant, customer.customer.Contact));

                        //send mail to the customer
                        Console.WriteLine(String.Format("reminder {0} from {1} your last video call is a long time ago  ", customer.customer.Klant, customer.customer.Contact));
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

