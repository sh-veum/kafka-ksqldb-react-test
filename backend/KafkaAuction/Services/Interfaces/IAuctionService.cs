using KafkaAuction.Dtos;
using KafkaAuction.Models;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;

namespace KafkaAuction.Services.Interfaces;

public interface IAuctionService
{
    Task<TablesResponse[]> CreateAuctionTableAsync(CancellationToken cancellationToken = default);
    Task<StreamsResponse[]> CreateAuctionBidStreamAsync(CancellationToken cancellationToken = default);
    Task<(HttpResponseMessage httpResponseMessage, AuctionDto? auctionDto)> InsertAuctionAsync(Auction auction);
    Task<(HttpResponseMessage httpResponseMessage, AuctionBidDto? auctionBidDto)> InsertBidAsync(Auction_Bid auctionBid);
    Task DropTablesAsync();
    Task<List<AuctionDto>> GetAllAuctions();
    Task<List<AuctionDto>> GetAuctions(int limit);
    Task<AuctionDto?> GetAuctionDtoById(string auction_id);
    Task<Auction?> GetAuctionById(string auction_id);
    Task<List<AuctionBidDto>> GetAllBids();
    Task<StreamsResponse[]> CreateAuctionsWithBidsStreamAsync(CancellationToken cancellationToken = default);
    Task<List<AuctionBidDto>> GetBidsForAuction(string auction_id);
    Task<List<AuctionBidMessageDto>> GetBidMessagesForAuction(string auction_id);
    Task<(HttpResponseMessage httpResponseMessage, Auction? auction)> DeleteAuction(string auction_id);
    Task<(HttpResponseMessage httpResponseMessage, AuctionDto? auctionDto)> EndAuctionAsync(string auction_id);

}