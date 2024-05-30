using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;

namespace KafkaAuction.Initializers;

public static class KsqlDbInitializer
{
    private const int numAuctions = 5;
    private const int numBidsPerAuction = 6;
    private const int numMessagesPerAuction = 8;

    public static async Task InitializeAsync(IAuctionService auctionService, IChatService chatService, IKsqlDbService ksqlDbService, ILogger logger)
    {
        TablesResponse[] tables = ksqlDbService.CheckTablesAsync().Result;
        StreamsResponse[] streams = ksqlDbService.CheckStreams().Result;
        logger.LogInformation("Tables Length: {Tables}", tables.Length);
        logger.LogInformation("Streams Length: {Streams}", streams.Length);

        await CreateAuctionTable(auctionService);
        await CreateChatMessageTable(chatService);

        await CreateAuctionBidStream(auctionService);
        await CreateAuctionWithBidsStream(auctionService);

        Thread.Sleep(5000);

        // Sleep for 5 seconds because creating tables is slow as hell

        var auctions = await auctionService.GetAllAuctions();
        if (auctions.Count == 0)
        {
            await InsertAuctionsAndBidsAndMessages(auctionService, chatService, numAuctions, numBidsPerAuction, numMessagesPerAuction);
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

    private static async Task CreateChatMessageTable(IChatService chatService)
    {
        await chatService.CreateChatTableAsync();
    }

    private static async Task InsertAuctionsAndBidsAndMessages(IAuctionService auctionService, IChatService chatService, int numAuctions, int numBidsPerAuction, int numMessagesPerAuction)
    {
        var auctions = new List<Auction>();

        // Generate auctions
        for (int i = 1; i <= numAuctions; i++)
        {
            var auctionId = Guid.NewGuid().ToString();
            var auction = new Auction
            {
                Auction_Id = auctionId,
                Title = $"Auction {i}",
                Description = $"Description for Auction {i}",
                Starting_Price = 1,
                Created_At = DateTime.UtcNow.AddSeconds(+i).ToString("yyyy-MM-dd HH:mm:ss"),
            };
            auctions.Add(auction);
            await auctionService.InsertAuctionAsync(auction);

            // Generate bids for this auction
            await InsertBidsForAuction(auctionService, auctionId, numBidsPerAuction);
            // Generate messages for this auction
            await InsertMessagesForAuction(chatService, auctionId, numMessagesPerAuction);
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
                Timestamp = DateTime.UtcNow.AddSeconds(+i).ToString("yyyy-MM-dd HH:mm:ss")
            };
            await auctionService.InsertBidAsync(auctionBid);
        }
    }

    private static async Task InsertMessagesForAuction(IChatService chatService, string auctionId, int numMessages)
    {
        for (int i = 1; i <= numMessages; i++)
        {
            var chatMessage = new Chat_Message
            {
                Message_Id = Guid.NewGuid().ToString(),
                Auction_Id = auctionId,
                Username = $"User{i}",
                MessageText = $"Message {i}",
                Timestamp = DateTime.UtcNow.AddSeconds(+i).ToString("yyyy-MM-dd HH:mm:ss")
            };
            await chatService.InsertMessageAsync(chatMessage);
        }
    }
}
