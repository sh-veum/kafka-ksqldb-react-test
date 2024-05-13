using KafkaAuction.Models;

namespace KafkaAuction.Services.Interfaces;

public interface IAuctionService
{
    Task<bool> CreateTablesAsync(CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> InsertAuctionAsync(Auction auction);
    Task<HttpResponseMessage> InsertBidAsync(Auction_Bid auctionBid);
    Task DropTablesAsync();
}