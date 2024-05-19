using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Serialization;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;

namespace KafkaAuction.Utilities;

public class TableCreator<T>
{
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly ILogger _logger;

    public TableCreator(IKSqlDbRestApiProvider restApiProvider, ILogger logger)
    {
        _restApiProvider = restApiProvider;
        _logger = logger;
    }

    public async Task<bool> CreateTableAsync(string tableName, CancellationToken cancellationToken = default)
    {
        var metadata = new EntityCreationMetadata(tableName)
        {
            Partitions = 1,
            Replicas = 1,
            ValueFormat = SerializationFormats.Json
        };

        var response = await _restApiProvider.CreateOrReplaceTableAsync<T>(metadata, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(content);
            return false;
        }

        return true;
    }

    public async Task<bool> CreateQueryableTableAsync(string tableName, CancellationToken cancellationToken = default)
    {
        var createQueryTableSql = $"CREATE TABLE QUERYABLE_{tableName.ToUpper()} AS SELECT * FROM {tableName.ToUpper()};";

        _logger.LogInformation("Generated SQL Statement: {SqlStatement}", createQueryTableSql);

        var ksqlDbStatement = new KSqlDbStatement(createQueryTableSql);

        var response = await _restApiProvider.ExecuteStatementAsync(ksqlDbStatement, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Error creating queryable table: {Content}", content);
            return false;
        }

        return true;
    }
}