// Copied from https://github.com/tomasfabian/ksqlDB.RestApi.Client-DotNet/blob/main/Samples/ksqlDB.RestApi.Client.Sample/Http/HttpClientFactory.cs

using IHttpClientFactory = ksqlDB.RestApi.Client.KSql.RestApi.Http.IHttpClientFactory;

namespace KafkaAuction.Http;

public class HttpClientFactory : IHttpClientFactory
{
    private readonly HttpClient httpClient;

    public HttpClientFactory(Uri uri)
    {
        if (uri == null)
            throw new ArgumentNullException(nameof(uri));

        httpClient = new HttpClient
        {
            BaseAddress = uri
        };
    }

    public HttpClient CreateClient()
    {
        return httpClient;
    }
}