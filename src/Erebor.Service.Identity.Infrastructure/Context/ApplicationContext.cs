﻿using Erebor.Service.Identity.Core.Interfaces;
using Erebor.Service.Identity.Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Erebor.Service.Identity.Infrastructure.Context
{
    public class ApplicationContext : IApplicationContext
    {
        private readonly IMongoDatabase _database = null;

        public ApplicationContext(IOptions<DataBaseSettings> settings)
        {
            var client = new MongoClient(settings.Value.ConnectionString);
            _database = client.GetDatabase(settings.Value.Database);

        }
        public IMongoCollection<User> Users => _database.GetCollection<User>("User");
        
    }
}
