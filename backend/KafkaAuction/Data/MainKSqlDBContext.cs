using ksqlDB.RestApi.Client.KSql.Config;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.Query.Context.Options;
using ksqlDB.RestApi.Client.KSql.Query.Options;
using ksqlDB.RestApi.Client.KSql.RestApi.Enums;

namespace KafkaAuction.Data;

public class MainKSqlDBContext : KSqlDBContext
{
    public MainKSqlDBContext(IConfiguration configuration) : base(BuildOptions(configuration))
    {
    }

    private static KSqlDBContextOptions BuildOptions(IConfiguration configuration)
    {
        var ksqlDbUrl = configuration.GetValue<string>("KSqlDb:Url");

        var optionsBuilder = new KSqlDbContextOptionsBuilder()
            .UseKSqlDb(ksqlDbUrl!)
            .SetBasicAuthCredentials("veum", "letmein")
            .SetJsonSerializerOptions(options =>
            {
                options.IgnoreReadOnlyFields = true;
            })
            //.SetAutoOffsetReset(AutoOffsetReset.Earliest) // global setting
            .SetProcessingGuarantee(ProcessingGuarantee.ExactlyOnce) // global setting
            .SetIdentifierEscaping(IdentifierEscaping.Keywords)
            .SetEndpointType(EndpointType.QueryStream) // uses HTTP/2.0
                                                       //.SetEndpointType(EndpointType.Query) // uses HTTP/1.0
            .SetupPushQuery(options =>
            {
                options.Properties[KSqlDbConfigs.KsqlQueryPushV2Enabled] = "true";
            })
            .SetupPullQuery(options =>
            {
                options[KSqlDbConfigs.KsqlQueryPullTableScanEnabled] = "true";
            });

        optionsBuilder.Options.DisposeHttpClient = false;
        return optionsBuilder.Options;
    }
}
