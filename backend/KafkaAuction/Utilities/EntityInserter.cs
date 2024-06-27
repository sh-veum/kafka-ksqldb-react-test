using System.Net;
using System.Reflection;
using System.Text;
using KafkaAuction.Services.Interfaces;
using ksqlDb.RestApi.Client.KSql.RestApi.Generators.Asserts;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;

namespace KafkaAuction.Utilities;

/// <summary>
/// Inserts an entity into a KSQL table or stream
/// </summary>
/// <typeparam name="T">The object class to insert</typeparam>
public class EntityInserter<T>
{
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly ILogger _logger;

    public EntityInserter(IKSqlDbRestApiProvider restApiProvider, ILogger logger)
    {
        _restApiProvider = restApiProvider;
        _logger = logger;
    }

    public async Task<HttpResponseMessage> InsertAsync(string tableName, T entity)
    {
        var topicExists = await _restApiProvider.AssertTopicExistsAsync(new AssertTopicOptions(tableName));

        if (topicExists.DefaultIfEmpty().First()?.Exists == false)
        {
            _logger.LogError("Topic {TopicName} does not exist", tableName);
            return new HttpResponseMessage(HttpStatusCode.NotFound);
        }

        var (columns, values) = FormatInsertValues(entity);

        var insertStatement = $"INSERT INTO {tableName} ({columns}) VALUES ({values});";
        _logger.LogInformation("InsertStatement: {Sql}", insertStatement);

        var ksqlDbStatement = new KSqlDbStatement(insertStatement);
        var result = await _restApiProvider.ExecuteStatementAsync(ksqlDbStatement);

        _logger.LogInformation("Execution result: {Result}", result);

        return result;
    }

    private (string, string) FormatInsertValues(T entity)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var columns = new StringBuilder();
        var values = new StringBuilder();

        foreach (var property in properties)
        {
            // Skip the RowTime property
            if (property.Name.Equals("RowTime", StringComparison.OrdinalIgnoreCase))
                continue;

            var value = property.GetValue(entity);
            if (value != null)
            {
                if (columns.Length > 0) columns.Append(", ");
                if (values.Length > 0) values.Append(", ");

                columns.Append(property.Name);
                values.Append(FormatValue(value));
            }
        }

        _logger.LogInformation("Columns: {Columns}", columns);
        _logger.LogInformation("Values: {Values}", values);

        return (columns.ToString(), values.ToString());
    }

    private static string? FormatValue(object value)
    {
        return value switch
        {
            DateTime dt => $"'{dt:yyyy-MM-dd HH:mm:ss}'",
            string str => $"'{str.Replace("'", "''")}'",
            string[] arr => $"ARRAY[{string.Join(", ", arr.Select(s => $"'{s.Replace("'", "''")}'"))}]",
            _ => value.ToString()
        };
    }
}