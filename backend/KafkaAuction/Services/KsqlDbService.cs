
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Extensions;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;
using ksqlDB.RestApi.Client.KSql.RestApi.Serialization;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Properties;

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

    public async Task<string?> DropSingleTablesAsync(string tableName)
    {
        var httpResult = await _restApiProvider.DropTableAndTopic(tableName);
        if (!httpResult.IsSuccessStatusCode)
        {
            var content = await httpResult.Content.ReadAsStringAsync();
            _logger.LogError(content);
            return null;
        }

        var responseContent = await httpResult.Content.ReadAsStringAsync();
        return responseContent;
    }

    public async Task<string?> DropSingleStreamAsync(string streamName)
    {
        var httpResult = await _restApiProvider.DropStreamAndTopic(streamName);
        if (!httpResult.IsSuccessStatusCode)
        {
            var content = await httpResult.Content.ReadAsStringAsync();
            _logger.LogError(content);
            return null;
        }

        var responseContent = await httpResult.Content.ReadAsStringAsync();
        return responseContent;
    }

    public async Task<TablesResponse[]> CheckTablesAsync()
    {
        return await _restApiProvider.GetTablesAsync();
    }

    public async Task<StreamsResponse[]> CheckStreams()
    {
        return await _restApiProvider.GetStreamsAsync();
    }
}