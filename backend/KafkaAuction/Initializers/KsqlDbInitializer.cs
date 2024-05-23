using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;

namespace KafkaAuction.Initializers;

public static class KsqlDbInitializer
{
    private const int numAuctions = 6;
    private const int numBidsPerAuction = 10;

    public static async Task InitializeAsync(IAuctionService auctionService)
    {
        await CreateAuctionTable(auctionService);
        await CreateAuctionBidStream(auctionService);
        await CreateAuctionWithBidsStream(auctionService);

        var auctions = await auctionService.GetAllAuctions();
        if (auctions.Count == 0)
        {

            await InsertAuctionsAndBids(auctionService, numAuctions, numBidsPerAuction);
        }
    }

    private static async Task CreateAuctionTable(IAuctionService auctionService)
    {
        await auctionService.CreateAuctionTableAsync();
    }

    private static async Task CreateAuctionBidStream(IAuctionService auctionService)
    {
        await auctionService.CreateAuctionBidStreamAsync();
    }

    private static async Task CreateAuctionWithBidsStream(IAuctionService auctionService)
    {
        await auctionService.CreateAuctionsWithBidsStreamAsync();
    }

    private static async Task InsertAuctionsAndBids(IAuctionService auctionService, int numAuctions, int numBidsPerAuction)
    {
        var auctions = new List<Auction>();

        // Generate auctions
        for (int i = 1; i <= numAuctions; i++)
        {
            var auctionId = Guid.NewGuid().ToString();
            var auction = new Auction { Auction_Id = auctionId, Title = $"Auction {i}" };
            auctions.Add(auction);
            await auctionService.InsertAuctionAsync(auction);

            // Generate bids for this auction
            await InsertBidsForAuction(auctionService, auctionId, numBidsPerAuction);
        }
    }

    private static async Task InsertBidsForAuction(IAuctionService auctionService, string auctionId, int numBids)
    {
        for (int i = 1; i <= numBids; i++)
        {
            var auctionBid = new Auction_Bid
            {
                Bid_Id = Guid.NewGuid().ToString(),
                Auction_Id = auctionId,
                Username = $"User{i}",
                Bid_Amount = i * 100,
                Timestamp = DateTime.UtcNow.ToString()
            };
            await auctionService.InsertBidAsync(auctionBid);
        }
    }
}
