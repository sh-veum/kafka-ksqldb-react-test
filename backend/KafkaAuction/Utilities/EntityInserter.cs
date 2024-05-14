using System.Net;
using System.Reflection;
using System.Text;
using Antlr4.Runtime.Misc;
using KafkaAuction.Services.Interfaces;
using ksqlDb.RestApi.Client.KSql.RestApi.Generators.Asserts;
using ksqlDb.RestApi.Client.KSql.RestApi.Responses.Asserts;
using ksqlDB.RestApi.Client.KSql.Query.Functions;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;

namespace KafkaAuction.Utilities;

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

        var (columns, values) = EntityInserter<T>.FormatInsertValues(entity);

        var insertStatement = $"INSERT INTO {tableName} ({columns}) VALUES ({values});";
        _logger.LogInformation("InsertStatement: {Sql}", insertStatement);

        var ksqlDbStatement = new KSqlDbStatement(insertStatement);
        var result = await _restApiProvider.ExecuteStatementAsync(ksqlDbStatement);

        return result;
    }

    private static (string, string) FormatInsertValues(T entity)
    {
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        var columns = new StringBuilder();
        var values = new StringBuilder();

        foreach (var property in properties)
        {
            var value = property.GetValue(entity);
            if (value != null)
            {
                if (columns.Length > 0) columns.Append(", ");
                if (values.Length > 0) values.Append(", ");

                columns.Append(property.Name);
                values.Append(EntityInserter<T>.FormatValue(value));
            }
        }

        return (columns.ToString(), values.ToString());
    }

    private static string? FormatValue(object value)
    {
        return value switch
        {
            string str => $"'{str.Replace("'", "''")}'",
            _ => value.ToString()
        };
    }
}