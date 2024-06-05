using System.Reflection;
using System.Text;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Annotations;

namespace KafkaAuction.Utilities;

/// <summary>
/// Placehodler since CreateOrReplaceStreamAsync and CreateOrReplaceTableAsync didnt work 
/// </summary>
public class StreamCreator<T>
{
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly ILogger _logger;

    public StreamCreator(IKSqlDbRestApiProvider restApiProvider, ILogger logger)
    {
        _restApiProvider = restApiProvider;
        _logger = logger;
    }

    public async Task<bool> CreateStreamAsync(string streamName, CancellationToken cancellationToken = default)
    {
        var createStreamSql = GenerateCreateStreamSql(streamName);
        _logger.LogInformation("Generated SQL Statement: {SqlStatement}", createStreamSql);

        var ksqlDbStatement = new KSqlDbStatement(createStreamSql);
        var response = await _restApiProvider.ExecuteStatementAsync(ksqlDbStatement, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Error creating stream: {Content}", content);
            return false;
        }

        return true;
    }

    private string GenerateCreateStreamSql(string streamName)
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
                columns.AppendLine($"{columnName} {columnType} KEY,");
            }
            else
            {
                columns.AppendLine($"{columnName} {columnType},");
            }
        }

        var columnsString = columns.ToString().TrimEnd(',', '\n', '\r');

        return $@"
            CREATE OR REPLACE STREAM {streamName} (
                {columnsString}
            ) WITH (
                KAFKA_TOPIC='{streamName}',
                PARTITIONS=1,
                VALUE_FORMAT='JSON',
                REPLICAS=1,
                RETENTION_MS=-1
            );";
    }
}