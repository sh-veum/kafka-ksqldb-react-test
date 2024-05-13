// Copied from https://github.com/tomasfabian/ksqlDB.RestApi.Client-DotNet/blob/main/Samples/ksqlDB.RestApi.Client.Sample/Providers/IKSqlDbRestApiProvider.cs

using ksqlDB.RestApi.Client.KSql.RestApi;

namespace KafkaAuction.Services.Interfaces;

public interface IKSqlDbRestApiProvider : IKSqlDbRestApiClient
{
    Task<HttpResponseMessage> DropStreamAndTopic(string streamName);
    Task<HttpResponseMessage> DropTableAndTopic(string tableName);
}