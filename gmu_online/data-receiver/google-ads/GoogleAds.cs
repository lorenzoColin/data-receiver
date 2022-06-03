using data_receiver.Models.ViewModels;
using Google.Ads.GoogleAds.Lib;
using Google.Ads.GoogleAds.V10.Services;
using Google.Ads.GoogleAds;
using Google.Ads.GoogleAds.Config;
using Google.Ads.GoogleAds.V10.Errors;
using Google.Api.Ads.AdWords.Lib;
using Google.Ads.Gax.Config;
using static Google.Ads.GoogleAds.V10.Resources.AccountBudget;

namespace data_receiver.google_ads
{
    public class GoogleAds
    {
        GoogleAdsConfig config { get; set; }
        GoogleAdsClient client { get; set; }

        public GoogleAds()
        {
             config = new GoogleAdsConfig()
            {
                DeveloperToken = "yBzwour2WR3cjnO6--vr7w",
                LoginCustomerId = "3743262179",
                OAuth2Mode = OAuth2Flow.APPLICATION,
                OAuth2ClientId = "515940014204-5o7gipoadae7vg70khnrk2qijrs7mmf9.apps.googleusercontent.com",
                OAuth2ClientSecret = "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",
                OAuth2RefreshToken = "1//09rXVRXziweZDCgYIARAAGAkSNwF-L9IrRfxuLR6OlDxxSyRa8umTp7fULugMkbZ-7beWPhb0X5wU81LqMZ3J7mVWYg-uFPW6-6E",
            };

            client = new GoogleAdsClient(config);
        }


        public  List<AccountListViewModel> customerList()
        {
            var accountslist = new List<AccountListViewModel>();




            //var adswordconfig = new AdWordsAppConfig
            //{
            //    DeveloperToken = "yBzwour2WR3cjnO6--vr7w",
            //    OAuth2ClientId = "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",
            //    OAuth2RefreshToken = "1//09rPlYi19UE5fCgYIARAAGAkSNwF-L9Ir7pKSkmp7OUsROXQn0KisD4l_0_wdtaS8HifT59R6z1_wDlthtZdeyZI1kUP5lVRhcMQ",
            //    ClientCustomerId = "3743262179",
            //    OAuth2ClientSecret = "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"
            //};


         
            //connectie api
            var googleAdsService = client.GetService(Services.V10.GoogleAdsService);


        

            string querycustomer = @"SELECT
                                    customer_client.client_customer,
                                    customer_client.level,
                                    customer_client.manager,
                                    customer_client.descriptive_name,
                                    customer_client.currency_code,
                                    customer_client.time_zone,
                                    customer_client.id
                                    FROM customer_client
                                    WHERE
                                    customer_client.level <= 1 AND customer_client.manager = false AND customer_client.status = ENABLED ";
            try
            {
                //alle klanten ophalen met naam en where op amslod
                //Issue a search request.

                 googleAdsService.SearchStream("3743262179", querycustomer,
                     delegate (SearchGoogleAdsStreamResponse resp)
                     {
                         foreach (var googleAdsRow in resp.Results)
                         {
                            //add the google ads customer id and the customer name to a list
                            accountslist.Add(new AccountListViewModel
                             {
                                 accountId = googleAdsRow.CustomerClient.Id,
                                 accountName = googleAdsRow.CustomerClient.DescriptiveName
                             });

                         }
                     }
                 );




                //coste van alle klanten 


                //hier plaats ik de lijst en doe ik where statement
                //dan pakt ie 1 klant uit de lijst wiens naam gelijk is aan de gebruiker die ik wil hebben
                //en dan zet ik de customer id in de searchstream
                //en dan krijg ik allec campagines en de koste daar van
                //dit tel ik bij elkaar op en dan heb ik de koste van 1 klant

                
                return accountslist;
                


            }
            catch (GoogleAdsException e)
            {
                Console.WriteLine("Failure:");
                Console.WriteLine($"Message: {e.Message}");
                Console.WriteLine($"Failure: {e.Failure}");
                Console.WriteLine($"Request ID: {e.RequestId}");
                throw;
            }
        }


        //get the campaign cost of the customer with the customerId
        public List<double> getcustomerWithCost(string customerId)
        {
          var datum =  DateTime.Now.ToString("yyyy-MM-dd");



            string querycost = $"SELECT campaign.name, campaign.status,customer.descriptive_name,metrics.cost_micros, metrics.average_cpc, metrics.average_cpm FROM campaign WHERE segments.date = '{datum}' AND campaign.status !=  REMOVED ";


            var googleAdsService = client.GetService(Services.V10.GoogleAdsService);
            var costlist = new List<double>();

            googleAdsService.SearchStream(customerId, querycost,
                delegate (SearchGoogleAdsStreamResponse resp)
                {
                    foreach (var googleAdsRow in resp.Results)
                    {
                        costlist.Add((double)googleAdsRow.Metrics.CostMicros / 1000000);
                    }
                }
            );


            return  costlist;
        }


    }

}

        

