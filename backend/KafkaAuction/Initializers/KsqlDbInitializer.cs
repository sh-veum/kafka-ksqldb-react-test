using KafkaAuction.Constants;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;
using Stream = ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams.Stream;

namespace KafkaAuction.Initializers;

public static class KsqlDbInitializer
{
    private const int numAuctions = 5;
    private const int numBidsPerAuction = 6;
    private const int numMessagesPerAuction = 8;

    public static async Task InitializeAsync(IAuctionService auctionService, IChatService chatService, IUserLocationService userLocationService, IKsqlDbService ksqlDbService, ILogger logger)
    {
        TablesResponse[] tablesResponse = await ksqlDbService.CheckTablesAsync();
        StreamsResponse[] streamsResponse = await ksqlDbService.CheckStreamsAsync();

        var tables = tablesResponse[0]?.Tables ?? [];
        var streams = streamsResponse[0]?.Streams ?? [];

        logger.LogInformation("Existing Tables: " + string.Join(", ", tables.Select(t => t.Name)));
        logger.LogInformation("Existing Streams: " + string.Join(", ", streams.Select(s => s.Name)));

        var createdAny = await EnsureTablesAndStreamsExist(auctionService, chatService, userLocationService, tables!, streams!, logger);

        if (createdAny)
        {
            logger.LogInformation("Waiting 5 seconds to allow ksqlDB to process the new tables and streams.");
            Thread.Sleep(5000);
        }

        var auctions = await auctionService.GetAllAuctions();
        if (auctions.Count == 0)
        {
            await InsertAuctionsAndBidsAndMessages(auctionService, chatService, numAuctions, numBidsPerAuction, numMessagesPerAuction);
        }
    }

    private static async Task<bool> EnsureTablesAndStreamsExist(IAuctionService auctionService, IChatService chatService, IUserLocationService userLocationService, Table[] tables, Stream[] streams, ILogger logger)
    {
        var createdAny = false;

        if (!tables.Any(t => t.Name == TableNameConstants.Auctions))
        {
            logger.LogInformation("{TableName} missing, creating new table.", TableNameConstants.Auctions);
            await auctionService.CreateAuctionTableAsync();
            createdAny = true;
        }

        if (!tables.Any(t => t.Name == TableNameConstants.ChatMessages))
        {
            logger.LogInformation("{TableName} missing, creating new table.", TableNameConstants.ChatMessages);
            await chatService.CreateChatTableAsync();
            createdAny = true;
        }

        if (!tables.Any(t => t.Name == TableNameConstants.UserLocation))
        {
            logger.LogInformation("{TableName} missing, creating new table.", TableNameConstants.UserLocation);
            await userLocationService.CreateUserLocationTableAsync();
            createdAny = true;
        }

        if (!streams.Any(s => s.Name == StreamNameConstants.AuctionBids))
        {
            logger.LogInformation("{StreamName} missing, creating new stream.", StreamNameConstants.AuctionBids);
            await auctionService.CreateAuctionBidStreamAsync();
            createdAny = true;
        }

        if (!streams.Any(s => s.Name == StreamNameConstants.AuctionWithBids))
        {
            logger.LogInformation("{StreamName} missing, creating new stream.", StreamNameConstants.AuctionWithBids);
            await auctionService.CreateAuctionsWithBidsStreamAsync();
            createdAny = true;
        }

        return createdAny;
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
                Current_Price = 1,
                Created_At = DateTime.UtcNow.AddSeconds(+i).ToString("yyyy-MM-dd HH:mm:ss"),
                End_Date = DateTime.UtcNow.AddHours(+i + 10).ToString("yyyy-MM-dd HH:mm:ss"),
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
                Message_Text = $"Message {i}",
                Created_Timestamp = DateTime.UtcNow.AddSeconds(+i).ToString("yyyy-MM-dd HH:mm:ss"),
            };
            await chatService.InsertMessageAsync(chatMessage);
        }
    }
}
