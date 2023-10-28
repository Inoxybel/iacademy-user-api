using Domain.Entities;
using Infra.Options;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infra;

public class DbContext
{
    public IMongoCollection<User> User { get; }
    public IMongoCollection<Company> Company { get; }
    public IMongoCollection<ActivationCode> ActivationCode { get; }
    public IMongoCollection<Plan> Plan { get; }
    public IMongoCollection<Feedback> Feedback { get; }

    protected DbContext() { }

    public DbContext(IOptions<DatabaseInstanceOptions> databaseInstanceOptions)
    {
        var client = new MongoClient(databaseInstanceOptions.Value.ConnectionString);

        var database = client.GetDatabase(databaseInstanceOptions.Value.Name);

        User = database.GetCollection<User>(nameof(User));
        Company = database.GetCollection<Company>(nameof(Company));
        ActivationCode = database.GetCollection<ActivationCode>(nameof(ActivationCode));
        Plan = database.GetCollection<Plan>(nameof(Plan));
        Feedback = database.GetCollection<Feedback>(nameof(Feedback));
    }
}
