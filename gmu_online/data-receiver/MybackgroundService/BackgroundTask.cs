using data_receiver.Data;
using data_receiver.ElasticConnection;
using data_receiver.Models;
using data_receiver.Models.ViewModels;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nest;
using System.Net;

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

        public async Task currentBudget()
        {
            ////service created om customers op te halen
            using var scope = _service.CreateScope();
            var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            ////mail scope
            using var test = _service.CreateScope();
            var Sendmail = test.ServiceProvider.GetRequiredService<IFluentEmail>();


            //count of the month
            int countDaysMonth = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
            //result 40 is the value of the user input
            IEnumerable<UserCustomerAction> userCustomerActions = _db.UserCustomerAction;
            var UserCustomerActionByDay = new List<UserCustomerActionByDay>();


            foreach (var action in userCustomerActions)
            {
                //this is the number of date 
                var result = (countDaysMonth / 100.0 * action.value);
                //add de dag en de usercustomerId
                UserCustomerActionByDay.Add(new UserCustomerActionByDay { day = Math.Round(result, MidpointRounding.AwayFromZero), usercustomerId = action.usercustomerId });
            }


            //day of today
            var day = DateTime.Now.Day;
            //this is going away
            var mail = new List<UserCustomerActionByDay>();

            foreach (var usercustomer in UserCustomerActionByDay)
            {

                if (day == usercustomer.day)
                {

                    var singleUs =  _db.UserCustomer.Find(usercustomer.usercustomerId);
                    var user = _db.Users.Find(singleUs.UserId);
                    var client = await _client.SearchAsync<Customer>(s => s.Query(s => s.Match(f => f.Field(f => f.Debiteurnr).Query(singleUs.DebiteurnrId))));
                    var customer = client.Documents.FirstOrDefault();



                    //bij die puntjes moeten nu nog berekening komen
                    //  var Adminemail = Sendmail
                    // .To(user.Email)
                    // .Subject("customer budget")
                    // .UsingTemplate("hi @Model.customer dit is je  @Model.Budget dit is wat je besteed heb heb ...... dus nu heb je .... besteed deze maand.", new { Email = user.Email, Customer = customer.Klant, Budget = customer.Max_budget });
                    //await Adminemail.SendAsync();


                    //dit is een test het kan weg later
                    mail.Add(usercustomer);
                }
            }
        }


        public  async Task LastVideoCall()
        {
            using var scope = _service.CreateScope();
            var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            ////mail scope
            using var test = _service.CreateScope();
            var Sendmail = test.ServiceProvider.GetRequiredService<IFluentEmail>();


            IEnumerable<UserCustomerAction> userCustomerActions = _db.UserCustomerAction;

            foreach (var usercustomerAction in userCustomerActions)
            {
                if(usercustomerAction.actionId == 3)
                {
                    // 12-05-20222

                    // 5 maanden

                    //12-05-2022

                    Console.WriteLine(usercustomerAction.value);

                }
            }

            var s = 2;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken )
        {



            //zo lang de app niet sluit
                while(!stoppingToken.IsCancellationRequested)
                {

                LastVideoCall();
                currentBudget();


                ////service created om customers op te halen
                ////customer scope
                using var scope = _service.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //var contacts = context.Contact;

                ////mail scope
                using var test = _service.CreateScope();
                var mail = test.ServiceProvider.GetRequiredService<IFluentEmail>();


                


                //////date time
                //var src = DateTime.Now.ToString("dd-MM-yyyy");

                //////loop over elke contact heen
                //foreach (var contact in contacts)
                //{
                //    var rawDate = DateTime.Parse(contact.birthdate);
                //    var correctDate = rawDate.ToString("dd-MM-yyyy");

                //    if (src == correctDate)
                //    {
                //    //    var email = mail
                //    //    .To("lorenzo8399@hotmail.com")
                //    //    .Subject("birthday")
                //    //    .UsingTemplate("hi @Model.Name this is your birthday", new { Name = contact.firstname });
                //    //    mail.Send();
                //    //    // log informatie om te testen of ie wel hierin komt
                //    //    _logger.LogInformation("customer {0} its your birthday {1}", contact.firstname, contact.birthdate);

                //        Console.WriteLine("{0} je bent jarig ", contact.email);
                //    }
                //    else
                //    {
                //        Console.WriteLine("jij bent niet jarig {0} datum: {1}  ",contact.email,contact.birthdate);
                //    }
                //}
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

