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
        //currentBudget is action id 1
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
                    var user = _db.Users.Find(singleUs.userid);
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
                mail.Add(usercustomer);


            }
        }

        //LastVideoCall is action id 2
        public  async Task LastVideoCall()
        {
            using var scope = _service.CreateScope();
            //database
            var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //mails
            var Sendmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();

            IEnumerable<UserCustomerAction> userCustomerActions = _db.UserCustomerAction;


            foreach (var usercustomerAction in userCustomerActions)
            {
                //if actionId 2 laaste video call gesprek
                //naam moet actionName worden
                if(usercustomerAction.actionId == 2)
                {
                var usercustomer = _db.UserCustomer.Find(usercustomerAction.usercustomerId);
                var user = await _db.Users.FindAsync(usercustomer.userid);
                var client = await _client.SearchAsync<Customer>(s => s.Query(s => s.Match(f => f.Field(f => f.Debiteurnr).Query(usercustomer.DebiteurnrId))));
                var customer = client.Documents.FirstOrDefault();


                    var soo = new List<string>();

                    //als customer niet null is en customer heeft een latest_video call propperty dan kom je in de if statement
                    if(customer != null && customer.Latest_videocall != "Geen behoefte aan bij de klant.")
                    {
                        //date of today
                        var Datenow = DateTime.Now.ToString("d/M/yyyy");

                        //date of now with datime type;
                        var DateOftoday =   DateTime.Parse(Datenow);

                        //dit is de laatste keer dat ze hebben gebeld
                        var Latest_videocall = customer.Latest_videocall;
                        var Latest_videocallDate = DateTime.Parse(Latest_videocall);



                        //this function gonne look at the latest_videcaldate and adds the value of the month to it
                        //het probleem met dit is als je laatste video call 2021 maart is en je vult 3 manden in krijg je em nooit binnen
                        //oplossing voor vinden...
                        var reminderMailDay = Latest_videocallDate.AddMonths(usercustomerAction.value).ToString("d/M/yyyy");
                        var remindermailtype = DateTime.Parse(reminderMailDay);

                        //dit is 2 maanden na de eerste reminder
                        var secondreminderMail = DateTime.Parse(reminderMailDay).AddDays(2).ToString("d/M/yyyy");;
                        var secondaryremindermailtype = DateTime.Parse(secondreminderMail);



                        //als de datum van nu gelijk is aan 2 dagen na de reminder of langer is dan 2 dagen na de reminder
                        //dan is deze propperty true
                        bool twoDaysafterReminder = DateOftoday == secondaryremindermailtype;



                        //als dit true is
                        if (twoDaysafterReminder == true)
                        {
                            //functie maken om de proppertys te verwijderen
                            //want ze ze zijn niet meer 
                            Console.WriteLine("verwijder deze values");
                            break;
                        }




                        //grab the date from now and add the months when you want to receive an email
                        //var reminderMailDay = DateOftoday.AddMonths(usercustomerAction.value).ToString("d/M/yyyy");


                        //if de date of now is equal to the remindermailDate
                        if (Datenow == reminderMailDay)
                        {
                            //send mail to the user 
                            Console.WriteLine(string.Format("reminder {0} plan your next video call in with the company {1} contact is {2} ", user.Email, customer.Klant, customer.Contact));
              
                            //send mail to the customer
                            Console.WriteLine(String.Format("reminder {0} from {1} your last video call is a long time ago  ", customer.Klant, customer.Contact));
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
            //database
            var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            //mails
            var Sendmail = scope.ServiceProvider.GetRequiredService<IFluentEmail>();


            IEnumerable<UserCustomerAction> userCustomerActions = _db.UserCustomerAction;


            foreach (var usercustomerAction in userCustomerActions)
            {
                //if actionId 3 laaste contact call gesprek
                if (usercustomerAction.actionId == 3)
                {
                    var usercustomer = _db.UserCustomer.Find(usercustomerAction.usercustomerId);
                    var user = await _db.Users.FindAsync(usercustomer.userid);
                    var client = await _client.SearchAsync<Customer>(s => s.Query(s => s.Match(f => f.Field(f => f.Debiteurnr).Query(usercustomer.DebiteurnrId))));
                    var customer = client.Documents.FirstOrDefault();

                    //dit is de laatste keer dat ze hebben gebeld
                    var Latest_contact = customer.Latest_contact;
                    var Latest_contactDate = DateTime.Parse(Latest_contact);

                    //grab the date from now and add the months when you want to receive an email
                    var reminderMailDay = Latest_contactDate.AddMonths(usercustomerAction.value).ToString("d/M/yyyy");

                    //date of today
                    var DateOftoday = DateTime.Now.ToString("d/M/yyyy");
                    var day = DateTime.Now;


                    //dit is 2 maanden na de eerste reminder
                    var secondreminderMail = DateTime.Parse(reminderMailDay).AddDays(2).ToString("d/M/yyyy"); ;
                    var secondaryremindermailtype = DateTime.Parse(secondreminderMail);



                    //als de datum van nu gelijk is aan 2 dagen na de reminder of langer is dan 2 dagen na de reminder
                    //dan is deze propperty true
                    bool twoDaysafterReminder = day == secondaryremindermailtype;


                    if (twoDaysafterReminder == true)
                    {
                        //functie maken om de proppertys te verwijderen
                        //want ze ze zijn niet meer 
                        Console.WriteLine("verwijder deze values");
                        break;
                    }



                    //if de date of now is equal to the remindermailDate
                    if (DateOftoday == reminderMailDay)
                    {
                        //send mail to the user 
                        Console.WriteLine(string.Format("reminder {0} plan your next video call in with the company {1} contact is {2} ", user.Email, customer.Klant, customer.Contact));

                        //send mail to the customer
                        Console.WriteLine(String.Format("reminder {0} from {1} your last video call is a long time ago  ", customer.Klant, customer.Contact));
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
                LastVideoCall();
                currentBudget();

                //////service created om customers op te halen
                //////customer scope
                //using var scope = _service.CreateScope();
                //var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                ////var contacts = context.Contact;

                //////mail scope
                //using var test = _service.CreateScope();
                //var mail = test.ServiceProvider.GetRequiredService<IFluentEmail>();


                


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

