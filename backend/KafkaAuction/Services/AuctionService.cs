
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Extensions;
using ksqlDB.RestApi.Client.KSql.RestApi.Serialization;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements.Properties;

namespace KafkaAuction.Services;

public class AuctionService : IAuctionService
{
    private readonly ILogger<AuctionService> _logger;
    public static readonly string AuctionTableName = "auctions";
    public static readonly string AuctionBidTableName = "auction_bids";
    private readonly IKSqlDbRestApiProvider _restApiProvider;

    public AuctionService(ILogger<AuctionService> logger, IKSqlDbRestApiProvider restApiProvider)
    {
        _logger = logger;
        _restApiProvider = restApiProvider;
    }

    public async Task<bool> CreateTablesAsync(CancellationToken cancellationToken = default)
    {
        // Create Auction table
        EntityCreationMetadata createAuctionTableMetadata = new(AuctionTableName)
        {
            Partitions = 1,
            Replicas = 1,
            ValueFormat = SerializationFormats.Json
        };

        var createAuctionTable = await _restApiProvider.CreateOrReplaceTableAsync<Auction>(createAuctionTableMetadata, cancellationToken);
        if (!createAuctionTable.IsSuccessStatusCode)
        {
            var content = await createAuctionTable.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(content);
        }

        // Create AuctionBid table
        var createAuctionBidTableSql = $@"CREATE OR REPLACE TABLE {AuctionBidTableName} (
            id INT PRIMARY KEY,
            auction_id INT,
            username VARCHAR,
            bid_amount DECIMAL(18,2)
        ) WITH (
            KAFKA_TOPIC='{AuctionBidTableName}',
            PARTITIONS=1,
            VALUE_FORMAT='JSON'
        );";

        var ksqlDbStatement = new KSqlDbStatement(createAuctionBidTableSql);
        var createAuctionBidTable = await _restApiProvider.ExecuteStatementAsync(ksqlDbStatement, cancellationToken);
        if (!createAuctionBidTable.IsSuccessStatusCode)
        {
            var content = await createAuctionBidTable.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError(content);
        }

        // EntityCreationMetadata createAuctionBidTableMetadata = new(AuctionBidTableName)
        // {
        //     Partitions = 1,
        //     Replicas = 1,
        //     ValueFormat = SerializationFormats.Json
        // };

        // var createAuctionBidTable = await _restApiProvider.CreateOrReplaceTableAsync<AuctionBid>(createAuctionBidTableMetadata, cancellationToken);
        // if (!createAuctionBidTable.IsSuccessStatusCode)
        // {
        //     var content = await createAuctionBidTable.Content.ReadAsStringAsync(cancellationToken);
        //     _logger.LogError(content);
        // }

        return true;
    }

    public async Task<HttpResponseMessage> InsertBidAsync(Auction_Bid auctionBid)
    {
        var insertStatement = _restApiProvider.ToInsertStatement(auctionBid);
        _logger.LogInformation("InsertStatement: {Sql}", insertStatement.Sql);

        var result = await _restApiProvider.InsertIntoAsync(auctionBid, new InsertProperties { ShouldPluralizeEntityName = true });

        return result;
    }

    public async Task<HttpResponseMessage> InsertAuctionAsync(Auction auction)
    {
        var insert =
          $"INSERT INTO {AuctionTableName} ({nameof(Auction.AuctionId)}, {nameof(Auction.Title)}) VALUES ({auction.AuctionId}, '{auction.Title}');";

        KSqlDbStatement ksqlDbStatement = new(insert);

        var result = await _restApiProvider.ExecuteStatementAsync(ksqlDbStatement);

        return result;
    }

    public async Task DropTablesAsync()
    {
        await _restApiProvider.DropTableAndTopic(AuctionTableName);
        await _restApiProvider.DropTableAndTopic(AuctionBidTableName);
    }

    public async Task<bool> DropSingleTablesAsync(string tableName)
    {
        var httpResult = await _restApiProvider.DropTableAndTopic(tableName);
        if (!httpResult.IsSuccessStatusCode)
        {
            var content = await httpResult.Content.ReadAsStringAsync();
            _logger.LogError(content);
        }

        return true;
    }

    public async Task<string?> CheckTablesAsync()
    {
        var statement = "SHOW TABLES;";
        var response = await _restApiProvider.ExecuteStatementAsync(new KSqlDbStatement(statement));
        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return content;
        }
        else
        {
            _logger.LogError("Failed to fetch tables: " + await response.Content.ReadAsStringAsync());
            return null;
        }
    }

}