using KafkaAuction.Models;

namespace KafkaAuction.Services.Interfaces;

public interface IAuctionService
{
    Task<List<string>?> CreateTablesAsync(CancellationToken cancellationToken = default);
    Task<List<string>?> CreateStreamsAsync(CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> InsertAuctionAsync(Auction auction);
    Task<HttpResponseMessage> InsertBidAsync(Auction_Bid auctionBid);
    Task DropTablesAsync();
}