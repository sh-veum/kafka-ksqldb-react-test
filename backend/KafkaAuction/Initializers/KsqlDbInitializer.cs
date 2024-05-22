using KafkaAuction.Services;

namespace KafkaAuction.Initializers;

public static class KsqlDbInitializer
{
    public static async Task InitializeAsync(AuctionService auctionService)
    {
        await CreateAuctionTable(auctionService);
        await CreateAuctionBidTable(auctionService);
    }

    private static async Task CreateAuctionTable(AuctionService auctionService)
    {
        await auctionService.CreateAuctionTableAsync();
    }

    private static async Task CreateAuctionBidTable(AuctionService auctionService)
    {
        await auctionService.CreateAuctionBidStreamAsync();
    }
}
