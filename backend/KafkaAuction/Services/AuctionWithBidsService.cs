using KafkaAuction.Constants;
using KafkaAuction.Dtos;
using KafkaAuction.Enums;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;

namespace KafkaAuction.Services;

/// <summary>
/// Service for handling messages containing information about both the auction and the bids in one stream.
/// </summary>
public class AuctionWithBidsService : IAuctionWithBidsService
{
    private readonly ILogger<AuctionWithBidsService> _logger;
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly KSqlDBContext _context;
    private readonly string _auctionsWithBidsStreamName = StreamNameConstants.AuctionWithBids;

    public AuctionWithBidsService(ILogger<AuctionWithBidsService> logger, IKSqlDbRestApiProvider restApiProvider, IConfiguration configuration)
    {
        _logger = logger;
        _restApiProvider = restApiProvider;

        var _ksqlDbUrl = configuration.GetValue<string>("KSqlDb:Url") ?? string.Empty;
        if (string.IsNullOrWhiteSpace(_ksqlDbUrl))
        {
            throw new InvalidOperationException("KSqlDb:Url configuration is missing");
        }

        var contextOptions = new KSqlDBContextOptions(_ksqlDbUrl)
        {
            ShouldPluralizeFromItemName = true
        };

        _context = new KSqlDBContext(contextOptions);
    }

    public async Task<StreamsResponse[]> CreateAuctionsWithBidsStreamAsync(CancellationToken cancellationToken = default)
    {
        string createStreamSql = $@"
            CREATE OR REPLACE STREAM {_auctionsWithBidsStreamName} AS
                SELECT
                    b.Auction_Id AS Auction_With_Bids_Id,
                    a.Title,
                    b.Bid_Id,
                    b.Username,
                    b.Bid_Amount,
                    b.Timestamp
                FROM
                    Auction_Bids b
                LEFT JOIN
                    Auctions a
                ON b.Auction_Id = a.Auction_Id
                EMIT CHANGES;";

        var ksqlDbStatement = new KSqlDbStatement(createStreamSql);
        var response = await _restApiProvider.ExecuteStatementAsync(ksqlDbStatement, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError($"Error creating {_auctionsWithBidsStreamName} stream: {content}");
        }

        _logger.LogInformation($"{_auctionsWithBidsStreamName} stream created successfully.");

        return await _restApiProvider.GetStreamsAsync(cancellationToken);
    }

    public async Task<DropResourceResponseDto> DropAuctionWithBidsStreamAsync()
    {
        var dropper = new KsqlResourceDropper(_restApiProvider, _logger);
        var response = await dropper.DropResourceAsync(_auctionsWithBidsStreamName, ResourceType.Stream);

        var dropResourceResponseDto = new DropResourceResponseDto
        {
            ResourceName = _auctionsWithBidsStreamName,
            IsSuccess = response.IsSuccessStatusCode
        };

        return dropResourceResponseDto;
    }

    public async Task<List<AuctionWithBidDto>> GetAllAuctionWithBidsAsync()
    {
        var auctionWithBids = _context.CreatePullQuery<Auction_With_Bids>("AUCTIONS_WITH_BIDS")
            .GetManyAsync();

        List<AuctionWithBidDto> auctionWithBidDtos = [];

        await foreach (var auctionWithBid in auctionWithBids.ConfigureAwait(false))
        {
            auctionWithBidDtos.Add(new AuctionWithBidDto
            {
                Auction_Id = auctionWithBid.Auction_With_Bids_Id,
                Bid_Id = auctionWithBid.Bid_Id,
                Title = auctionWithBid.Title,
                Username = auctionWithBid.Username,
                Bid_Amount = auctionWithBid.Bid_Amount,
                Timestamp = auctionWithBid.Timestamp,
            });
        }

        return auctionWithBidDtos;
    }
}
