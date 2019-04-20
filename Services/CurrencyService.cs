using Models;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;

namespace Services
{
    class CurrencyService
    {
        private readonly IMongoCollection<CurrencyModel> _currencies;

        public CurrencyService(IConfiguration config, string collectionName)
        {
            var client = new MongoClient(config.GetConnectionString("CurrenciesDb"));
            var database = client.GetDatabase("currencies");
            _currencies = database.GetCollection<CurrencyModel>(collectionName);
        }

        public List<CurrencyModel> Get()
        {
            return _currencies.Find(x => true).ToList();
        }

        public CurrencyModel GetById(string id)
        {
            return _currencies.Find<CurrencyModel>(c => c.Id == id).FirstOrDefault();
        }

        public CurrencyModel Create(CurrencyModel currency)
        {
            _currencies.InsertOne(currency);
            return currency;
        }
    }
}
