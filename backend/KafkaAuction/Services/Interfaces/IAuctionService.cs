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
    Task<AuctionDto?> GetAuctionById(string auction_id);
    Task<Auction?> GetAuction(string auction_id);
    Task<List<AuctionBidDtoWithTimeStamp>> GetAllBids();
    Task<StreamsResponse[]> CreateAuctionsWithBidsStreamAsync(CancellationToken cancellationToken = default);
    Task<List<AuctionBidDtoWithTimeStamp>> GetBidsForAuction(string auction_id);
    Task<List<AuctionBidMessageDto>> GetBidMessagesForAuction(string auction_id);
}