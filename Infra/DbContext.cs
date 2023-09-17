﻿using Domain.Entities;
using Infra.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infra;

public class DbContext
{
    public IMongoCollection<User> User { get; }

    protected DbContext() { }

    public DbContext(IOptions<DatabaseInstanceOptions> databaseInstanceOptions)
    {
        var client = new MongoClient(databaseInstanceOptions.Value.ConnectionString);

        var database = client.GetDatabase(databaseInstanceOptions.Value.Name);

        User = database.GetCollection<User>(nameof(User));
    }
}