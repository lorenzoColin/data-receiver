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
using System.Transactions;
using Microsoft.EntityFrameworkCore;

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
            var client = new ElasticSearchClient().EsClient();
            var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var sendmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();
            var GoogleAds = new GoogleAds();


            //count of the month
            int countDaysMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            int day = DateTime.Now.Month;


            //result 40 is the value of the user input
            var userCustomerActions = _db.UserCustomerAction.ToList();

           

            //hier loopt ie over een lijst met dagen dat zijn ingevuld door de gebruiker
            foreach (var UserCustomerActionByDay in userCustomerActions)
            {
                if (UserCustomerActionByDay.actionId == 1)
                {
                    var usercustomer = _db.UserCustomer.Find(UserCustomerActionByDay.usercustomerId);
                    var UserId = usercustomer.userid;
                    var User = _db.Users.Find(UserId);
                    var allcustomer = new CustomerList(_db);
                    var customers = allcustomer.getallcustomers();

                    
               
                    var singlecustomer = customers.Where(s => s.customer.CustomerType == usercustomer.customerType && s.customer.Debiteurnr == usercustomer.DebiteurnrId).FirstOrDefault();
                    var AdsAccountName = singlecustomer.customer.Ads.Trim();

                    var customerlist = GoogleAds.customerList();


                    //krijg de klanten naam met customer id            
                    var singelist = customerlist.Where(s => s.accountName == AdsAccountName).FirstOrDefault();
                    if (singelist != null)
                    {
                        var cost = Math.Round(GoogleAds.getcustomerWithCost(singelist.accountId.ToString()).Sum(), 2);

                        /* 1.search in the usercustomeraction for the specific user with the value */
                        var SingleUserCustomerAction = _db.UserCustomerAction.Where(s => s.usercustomerId == UserCustomerActionByDay.usercustomerId && s.value == UserCustomerActionByDay.value && s.action.id == 1).FirstOrDefault();

                        //sla koste op in database
                        if (SingleUserCustomerAction != null)
                        {
                            //if the user hase not cost yet add the cost
                            if (SingleUserCustomerAction.cost == null)
                            {

                                UserCustomerActionByDay.cost = cost;
                                _db.SaveChanges();
                            }
                            //if the user already has a cost update em with the cost of the next day
                            if (SingleUserCustomerAction.cost != null)
                            {
                                var totalCost = cost + SingleUserCustomerAction.cost;
                                var round = Math.Round((double)totalCost, 2, MidpointRounding.AwayFromZero);

                                SingleUserCustomerAction.cost = round;
                                _db.SaveChanges();
                            }
                            //if the date of today is equal to when a user wants to receive a mail
                            //send a mail with the cost of the month
                            if (day == UserCustomerActionByDay.value)
                            {
                                var totalCost = cost + SingleUserCustomerAction.cost;
                                var round = Math.Round((double)totalCost, 2, MidpointRounding.AwayFromZero);

                                var email = sendmail
                               .To(User.Email)
                               .Subject("Current budget")
                               .UsingTemplate("Good day @Model.Name. The max budget of @Model.customer for this month is €@Model.budget of which €@Model.cost is already spend."
                               , new { Name = User.FirstName, customer = singlecustomer.customer.Klant, budget = singlecustomer.customer.Max_budget, cost = round });
                                email.Send();
                                //send a mail with cost and max budget

                                //when the mail is send the total cost is going to be zero again
                                SingleUserCustomerAction.cost = null;
                                _db.SaveChanges();
                            }
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
              
                //actionId 2 is last video call
                if(usercustomerAction.actionId == 2)
                {
                    var usercustomer = _db.UserCustomer.Find(usercustomerAction.usercustomerId);
                    var allcustomer = new CustomerList(_db);
                    var getallcustomers = allcustomer.getallcustomers();




                    var customer = getallcustomers.Where(s => s.customer.CustomerType == usercustomer.customerType && s.customer.Debiteurnr == usercustomer.DebiteurnrId).First();
                    var user = _db.Users.Find(usercustomer.userid);


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

                            var email = sendmail
                           .To(user.Email)
                           .Subject("reminder last video call")
                           .UsingTemplate("hi @Model.Name your last video call with @Model.contact from @Model.customer was @Model.monthsAgo  ",
                           new { Name = user.FirstName, contact = customer.customer.Contact,customer = customer.customer.Klant,monthsAgo = usercustomerAction.value });
                           await email.SendAsync();
                                        

                        } 

                    }
                }
            }
            await Task.CompletedTask;
        }

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


                    //if the date of the latest contsact is bigger than today remove a year from the date
                    if (Latest_contactDate > day)
                    {
                        Latest_contactDate = Latest_contactDate.AddYears(-1);
                    }

                    //if de date of now is equal to the remindermailDate
                    if (DateOftoday == reminderMailDay)
                    {

                        var email = sendmail
                       .To(user.Email)
                       .Subject("reminder last contact")
                       .UsingTemplate("hi @Model.Name your last contact with @Model.contact from @Model.customer was @Model.monthsAgo  ",
                       new { Name = user.FirstName, contact = customer.customer.Contact, customer = customer.customer.Klant, monthsAgo = usercustomerAction.value });
                        await email.SendAsync();

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
                LastContact();
                LastVideoCall();
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

