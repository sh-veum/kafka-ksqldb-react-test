// Copied from https://github.com/tomasfabian/ksqlDB.RestApi.Client-DotNet/blob/main/Samples/ksqlDB.RestApi.Client.Sample/Providers/KSqlDbRestApiProvider.cs

using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi;
using ksqlDB.RestApi.Client.KSql.RestApi.Http;
using IHttpClientFactory = ksqlDB.RestApi.Client.KSql.RestApi.Http.IHttpClientFactory;

namespace KafkaAuction.Services;

public class KSqlDbRestApiProvider : KSqlDbRestApiClient, IKSqlDbRestApiProvider
{
    private readonly IConfiguration _configuration;
    private static string? KsqlDbUrl;

    public KSqlDbRestApiProvider(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILoggerFactory? loggerFactory = null)
      : base(httpClientFactory, loggerFactory)
    {
        _configuration = configuration;
        KsqlDbUrl = _configuration.GetValue<string>("KSqlDb:Url")!;
    }

    public Task<HttpResponseMessage> DropStreamAndTopic(string streamName)
    {
        return DropStreamAsync(streamName, true, true);
    }

    public Task<HttpResponseMessage> DropTableAndTopic(string tableName)
    {
        return DropTableAsync(tableName, true, true);
    }

    public static KSqlDbRestApiProvider Create(string? ksqlDbUrl = null)
    {
        var uri = new Uri(ksqlDbUrl ?? KsqlDbUrl ?? throw new ArgumentNullException(nameof(ksqlDbUrl)));

        var httpClient = new HttpClient
        {
            BaseAddress = uri
        };

        return new KSqlDbRestApiProvider(new HttpClientFactory(httpClient), new ConfigurationBuilder().Build());
    }
}