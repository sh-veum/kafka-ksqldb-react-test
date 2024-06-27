using KafkaAuction.Dtos;
using KafkaAuction.Models;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;

namespace KafkaAuction.Services.Interfaces;

public interface IAuctionBidService
{
    Task<StreamsResponse[]> CreateAuctionBidStreamAsync(CancellationToken cancellationToken = default);
    Task<(HttpResponseMessage httpResponseMessage, AuctionBidDto? auctionBidDto)> InsertBidAsync(Auction_Bid auctionBid);
    Task<DropResourceResponseDto> DropBidStreamAsync();
    Task<List<AuctionBidDto>> GetAllBidsAsync();
    Task<List<AuctionBidDto>> GetBidsForAuctionAsync(string auction_id);
    Task<List<AuctionBidMessageDto>> GetBidMessagesForAuctionAsync(string auction_id);
}