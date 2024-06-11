// Modified code form https://github.com/tomasfabian/ksqlDB.RestApi.Client-DotNet/blob/main/Tests/ksqlDB.RestApi.Client.Tests/Mocking/MockingExamples.cs

using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.RestApi;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;

namespace KafkaAuction.Tests.Mocking;

public class TestableKSqlDBContext : KSqlDBContext
{
    public TestableKSqlDBContext(string ksqlDbUrl) : base(ksqlDbUrl)
    {
    }

    public TestableKSqlDBContext(KSqlDBContextOptions contextOptions) : base(contextOptions)
    {
    }

    public readonly Mock<IKSqlDbProvider> KSqlDbProviderMock = new Mock<IKSqlDbProvider>();

    protected override void OnConfigureServices(IServiceCollection serviceCollection, KSqlDBContextOptions contextOptions)
    {
        serviceCollection.TryAddScoped<IKSqlDbProvider>(c => KSqlDbProviderMock.Object);

        base.OnConfigureServices(serviceCollection, contextOptions);
    }
}
