using data_receiver.Data;
using FluentEmail.Core;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Net;

namespace data_receiver.MybackgroundService
{
    public class BackgroundTask : BackgroundService
    {
      
        private readonly ILogger<BackgroundTask>  _backgroundTask;
        private readonly IServiceProvider _service;



        public BackgroundTask( ILogger<BackgroundTask> backgroundTask, IServiceProvider services)
        {
            _service = services;
            _backgroundTask = backgroundTask;
          
        }
        //dit is de functie die draaid zodra de app aan staat 
        protected override Task ExecuteAsync(CancellationToken stoppingToken )
        {
            

            Console.WriteLine("het werkt");

            //service created om customers op te halen
            using var scope = _service.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var customers = context.Customer;

            //mail scope
            using var test = _service.CreateScope();
            var mail = test.ServiceProvider.GetRequiredService<IFluentEmail>();

            

            //loop functie
            foreach (var customer in customers)
            {
                if(customer.birthdate == "08-04-1999")
                {
                    var email = mail
              .To("lorenzo8399@hotmail.com")
              .Subject("Test email")
          .UsingTemplate("hi @Model.Name this is the first email @(5 + 5)!", new { Name = "test name" });
                    mail.Send();

                    Console.WriteLine(customer.firstname, email);
                }
            }

           
            






            return Task.CompletedTask;   
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
