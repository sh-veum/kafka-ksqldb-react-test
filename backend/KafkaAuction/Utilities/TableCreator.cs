using System.Reflection;
using System.Text;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Serialization;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Utilities;

/// <summary>
/// Placeholder since CreateOrReplaceStreamAsync and CreateOrReplaceTableAsync didn't work 
/// </summary>
public class TableCreator<T>
{
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly ILogger _logger;

    public TableCreator(IKSqlDbRestApiProvider restApiProvider, ILogger logger)
    {
        _restApiProvider = restApiProvider ?? throw new ArgumentNullException(nameof(restApiProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    // public async Task<bool> CreateTableAsync(string tableName, CancellationToken cancellationToken = default)
    // {
    //     var metadata = new EntityCreationMetadata(tableName)
    //     {
    //         Partitions = 1,
    //         Replicas = 1,
    //         ValueFormat = SerializationFormats.Json
    //     };

    //     var response = await _restApiProvider.CreateOrReplaceTableAsync<T>(metadata, cancellationToken);
    //     if (!response.IsSuccessStatusCode)
    //     {
    //         var content = await response.Content.ReadAsStringAsync(cancellationToken);
    //         _logger.LogError(content);
    //         return false;
    //     }

    //     return true;
    // }

    public async Task<bool> CreateTableAsync(string tableName, CancellationToken cancellationToken = default)
    {
        var createTableSql = GenerateCreateTableSql(tableName);
        _logger.LogInformation("Generated SQL Statement: {SqlStatement}", createTableSql);

        var ksqlDbStatement = new KSqlDbStatement(createTableSql);
        var response = await _restApiProvider.ExecuteStatementAsync(ksqlDbStatement, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Error creating table: {Content}", content);
            return false;
        }

        return true;
    }

    private string GenerateCreateTableSql(string tableName)
    {
        var properties = typeof(T).GetProperties();
        _logger.LogInformation("Properties: {Properties}", properties);
        var columns = new StringBuilder();

        foreach (var property in properties)
        {
            var columnName = property.Name.ToLower();
            var columnType = TypeMapper.GetKSqlType(property.PropertyType);

            // Exclude unwanted properties
            // if (
            //     columnName == "headers" ||
            //     columnName == "rowoffset" ||
            //     columnName == "rowpartition" ||
            //     columnName == "rowtime"
            //     )
            // {
            //     continue;
            // }

            if (property.GetCustomAttribute<KeyAttribute>() != null)
            {
                columns.AppendLine($"{columnName} {columnType} PRIMARY KEY,");
            }
            else
            {
                columns.AppendLine($"{columnName} {columnType},");
            }
        }

        var columnsString = columns.ToString().TrimEnd(',', '\n', '\r');

        return $@"
            CREATE OR REPLACE TABLE {tableName} (
                {columnsString}
            ) WITH (
                KAFKA_TOPIC='{tableName}',
                PARTITIONS=1,
                REPLICAS=1,
                VALUE_FORMAT='JSON'
            );";
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