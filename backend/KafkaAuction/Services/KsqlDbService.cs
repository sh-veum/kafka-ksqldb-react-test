using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;

namespace KafkaAuction.Services;

public class KsqlDbService : IKsqlDbService
{
    private readonly ILogger<KsqlDbService> _logger;
    private readonly IKSqlDbRestApiProvider _restApiProvider;

    public KsqlDbService(ILogger<KsqlDbService> logger, IKSqlDbRestApiProvider restApiProvider)
    {
        _logger = logger;
        _restApiProvider = restApiProvider;
    }

    public async Task<HttpResponseMessage> DropSingleTablesAsync(string tableName)
    {
        var httpResult = await _restApiProvider.DropTableAndTopic(tableName);
        if (!httpResult.IsSuccessStatusCode)
        {
            var content = await httpResult.Content.ReadAsStringAsync();
            _logger.LogError(content);
        }
        return httpResult;
    }

    public async Task<HttpResponseMessage> DropSingleStreamAsync(string streamName)
    {
        var httpResult = await _restApiProvider.DropStreamAndTopic(streamName);
        if (!httpResult.IsSuccessStatusCode)
        {
            var content = await httpResult.Content.ReadAsStringAsync();
            _logger.LogError(content);
        }
        return httpResult;
    }

    public async Task<bool> CreateSingleTableAsync<T>(string tableName, CancellationToken cancellationToken = default)
    {
        var tableCreator = new TableCreator<T>(_restApiProvider, _logger);

        if (!await tableCreator.CreateTableAsync(tableName, cancellationToken))
        {
            throw new InvalidOperationException($"Failed to create {tableName} table");
        }

        return true;
    }

    public async Task<TablesResponse[]> CheckTablesAsync()
    {
        return await _restApiProvider.GetTablesAsync();
    }

    public async Task<StreamsResponse[]> CheckStreamsAsync()
    {
        return await _restApiProvider.GetStreamsAsync();
    }
}