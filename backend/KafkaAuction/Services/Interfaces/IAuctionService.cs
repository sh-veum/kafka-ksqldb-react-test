using KafkaAuction.Dtos;
using KafkaAuction.Models;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;

namespace KafkaAuction.Services.Interfaces;

public interface IAuctionService
{
    Task<TablesResponse[]> CreateAuctionTableAsync(CancellationToken cancellationToken = default);
    Task<StreamsResponse[]> CreateAuctionBidStreamAsync(CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> InsertAuctionAsync(Auction auction);
    Task<HttpResponseMessage> InsertBidAsync(Auction_Bid auctionBid);
    Task DropTablesAsync();
    Task<List<AuctionDto>> GetAllAuctions();
    Task<List<AuctionDto>> GetAuctions(int limit);
    Task<List<AuctionBidDto>> GetAllBids();
    Task<HttpResponseMessage> CreateAuctionsWithBidsStreamAsync(CancellationToken cancellationToken = default);
}