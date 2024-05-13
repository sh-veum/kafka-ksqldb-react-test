
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Extensions;
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

    public async Task<bool> DropSingleTablesAsync(string tableName)
    {
        var httpResult = await _restApiProvider.DropTableAndTopic(tableName);
        if (!httpResult.IsSuccessStatusCode)
        {
            var content = await httpResult.Content.ReadAsStringAsync();
            _logger.LogError(content);
        }

        return true;
    }

    public async Task<string?> CheckTablesAsync()
    {
        var statement = "SHOW TABLES;";
        var response = await _restApiProvider.ExecuteStatementAsync(new KSqlDbStatement(statement));
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        else
        {
            _logger.LogError("Failed to fetch tables: " + await response.Content.ReadAsStringAsync());
            return null;
        }
    }

    public async Task<string?> MakeSQLQueryAsync(string query)
    {
        var statement = new KSqlDbStatement(query);
        var response = await _restApiProvider.ExecuteStatementAsync(statement);
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogError($"Failed to execute query: {errorContent}");
            return errorContent; // Directly returning the error message
        }

        return await response.Content.ReadAsStringAsync();
    }
}