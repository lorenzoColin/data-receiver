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

                //service created om customers op te halen
                using var scope = _service.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var customers = context.Customer;

                //mail scope
                using var test = _service.CreateScope();
                var mail = test.ServiceProvider.GetRequiredService<IFluentEmail>();


                //date time
                var src = DateTime.Now;
                var time = src.ToLongTimeString();
                var NowTime = src.ToShortTimeString();
                var nowDate = src.ToShortDateString();

                //loop over elke customer
                foreach(var customer in customers)
                {
                    //check of birtday gelijk is aan datum van nu
                    if(customer.birthdate == nowDate)
                    {
                        //zo ja stuur een mail
                        var email = mail
                        .To("lorenzo8399@hotmail.com")
                        .Subject("Test email")
                        .UsingTemplate("hi @Model.Name this is the first email @(5 + 5)!", new { Name = "test name" });
                        mail.Send();
                        // log informatie om te testen of ie wel hierin komt
                        _logger.LogInformation("customer {0} its your birthday {1}", customer.firstname,customer.birthdate);

                    }
                }

                    //hij loopt op dit mommemt om de 5 seconden dit moet veranderd worden naar
                    // dat ie om de dag checkt
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
