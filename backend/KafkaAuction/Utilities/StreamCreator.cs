using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Serialization;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;

namespace KafkaAuction.Utilities;

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
        var createStreamSql = $@"
            CREATE OR REPLACE STREAM {streamName} (
                auction_id INT KEY,
                username VARCHAR,
                bid_amount DECIMAL(18,2)
            ) WITH (
                KAFKA_TOPIC='{streamName}',
                PARTITIONS=1,
                VALUE_FORMAT='JSON'
            );";

        var ksqlDbStatement = new KSqlDbStatement(createStreamSql);
        var response = await _restApiProvider.ExecuteStatementAsync(ksqlDbStatement, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(content);
            return false;
        }

        return true;
    }
}