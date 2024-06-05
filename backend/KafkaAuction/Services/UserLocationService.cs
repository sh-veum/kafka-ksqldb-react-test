using KafkaAuction.Enums;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.Linq.PullQueries;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;

public class UserLocationService : IUserLocationService
{
    private readonly ILogger<UserLocationService> _logger;
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly KSqlDBContext _context;
    private readonly string _userLocationStreamName = "USER_LOCATIONS";

    public UserLocationService(ILogger<UserLocationService> logger, IKSqlDbRestApiProvider restApiProvider, IConfiguration configuration)
    {
        _logger = logger;
        _restApiProvider = restApiProvider;

        var _ksqlDbUrl = configuration.GetValue<string>("KSqlDb:Url") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(_ksqlDbUrl))
        {
            throw new InvalidOperationException("KSqlDb:Url configuration is missing");
        }

        var contextOptions = new KSqlDBContextOptions(_ksqlDbUrl)
        {
            ShouldPluralizeFromItemName = true
        };

        _context = new KSqlDBContext(contextOptions);
    }

    public async Task<TablesResponse[]> CreateUserLocationTableAsync(CancellationToken cancellationToken = default)
    {
        var userLocationTableCreator = new TableCreator<User_Location>(_restApiProvider, _logger);
        if (!await userLocationTableCreator.CreateTableAsync(_userLocationStreamName, cancellationToken))
        {
            throw new InvalidOperationException("Failed to create table");
        }

        await userLocationTableCreator.CreateQueryableTableAsync(_userLocationStreamName, cancellationToken);

        return await _restApiProvider.GetTablesAsync(cancellationToken);
    }

    public async Task<HttpResponseMessage> InsertUserLocationAsync(User_Location userLocation)
    {
        var inserter = new EntityInserter<User_Location>(_restApiProvider, _logger);
        return await inserter.InsertAsync(_userLocationStreamName, userLocation);
    }

    public async Task DropTablesAsync()
    {
        var dropper = new KsqlResourceDropper(_restApiProvider, _logger);
        await dropper.DropResourceAsync(_userLocationStreamName, ResourceType.Stream);
        await dropper.DropResourceAsync("QUERYABLE_" + _userLocationStreamName, ResourceType.Table);
    }

    public async Task<List<User_Location>?> GetUsersOnPage(string page)
    {
        var query = _context.CreatePullQuery<User_Location>($"QUERYABLE_{_userLocationStreamName}")
            .Where(c => c.Pages.Contains(page))
            .GetManyAsync();

        if (query == null)
        {
            return null;
        }

        return await query.ToListAsync();
    }

    public async Task<List<User_Location>?> GetAllUserLocations()
    {
        var query = _context.CreatePullQuery<User_Location>($"QUERYABLE_{_userLocationStreamName}")
            .GetManyAsync();

        if (query == null)
        {
            return null;
        }

        return await query.ToListAsync();
    }
}