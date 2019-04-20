using Microsoft.Extensions.Configuration;
using Models;
using Newtonsoft.Json;
using Services;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CurrencyFetch
{
    class Program
    {
        private static readonly HttpClient client = new HttpClient();
        
        private readonly IConfiguration configurationBuilder;
        private readonly CurrencyService currencyService;
        private readonly string currencyType = "USD";

        public Program()
        {
            configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Config/appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            currencyService = new CurrencyService(configurationBuilder, currencyType.ToLower());
        }
        static void Main(string[] args)
        {
            Program p = new Program();
            p.Start();
        }

        private void Start()
        {
            ProcessCurrencies().Wait();
        }

        private async Task ProcessCurrencies()
        {
            try
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-IBM-client-id", configurationBuilder["ClientId"]);
                client.DefaultRequestHeaders.Add("X-IBM-client-secret", configurationBuilder["ClientSecret"]);

                var clientResponse = await client.GetAsync(configurationBuilder["ClientUrl"] + currencyType + "/?date=" + DateTime.Now.ToShortDateString());
                if (clientResponse.IsSuccessStatusCode)
                {
                    dynamic response = JsonConvert.DeserializeObject(clientResponse.Content.ReadAsStringAsync().Result);

                    CurrencyModel model = new CurrencyModel();
                    model.RateBuy = response.data.rate_buy;
                    model.RateSell = response.data.rate_sell;

                    var serviceResponse = currencyService.Create(model);
                    if (string.IsNullOrEmpty(serviceResponse.Id))
                    {
                        Console.WriteLine("Write operation failed");
                    }

                    Console.WriteLine("Success");
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }
    }
}
