using data_receiver.Data;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace data_receiver.MybackgroundService
{
    public class BackgroundTask : BackgroundService
    {

      
        private readonly ILogger<BackgroundTask>  _logger;
        private readonly IServiceProvider _service;

        public BackgroundTask( ILogger<BackgroundTask> logger, IServiceProvider services)
        {
            _service = services;
            _logger = logger;
          
        }
        //dit is de functie die draaid zodra de app aan staat 
        protected override async Task ExecuteAsync(CancellationToken stoppingToken )
        {
            //zo lang de app niet sluit
                while(!stoppingToken.IsCancellationRequested)
                {

                ////service created om customers op te halen
                ////customer scope
                using var scope = _service.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var contacts = context.Contact;

                ////mail scope
                using var test = _service.CreateScope();
                var mail = test.ServiceProvider.GetRequiredService<IFluentEmail>();

                ////date time
                var src = DateTime.Now.ToString("dd-MM-yyyy");

                ////loop over elke contact heen
                foreach (var contact in contacts)
                {
                    var rawDate = DateTime.Parse(contact.birthdate);
                    var correctDate = rawDate.ToString("dd-MM-yyyy");

                    if (src == correctDate)
                    {
                    //    var email = mail
                    //    .To("lorenzo8399@hotmail.com")
                    //    .Subject("birthday")
                    //    .UsingTemplate("hi @Model.Name this is your birthday", new { Name = contact.firstname });
                    //    mail.Send();
                    //    // log informatie om te testen of ie wel hierin komt
                    //    _logger.LogInformation("customer {0} its your birthday {1}", contact.firstname, contact.birthdate);

                        Console.WriteLine("{0} je bent jarig ", contact.email);
                    }
                    else
                    {
                        Console.WriteLine("jij bent niet jarig {0} datum: {1}  ",contact.email,contact.birthdate);
                    }
                
                }
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
