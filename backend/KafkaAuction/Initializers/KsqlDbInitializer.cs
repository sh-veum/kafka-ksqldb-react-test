using KafkaAuction.Constants;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;
using Stream = ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams.Stream;

namespace KafkaAuction.Initializers;

public static class KsqlDbInitializer
{
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
            await InsertAuctionsAndBidsAndMessages(auctionService, chatService);
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

    private static async Task InsertAuctionsAndBidsAndMessages(IAuctionService auctionService, IChatService chatService)
    {
        var auctions = new List<Auction>
        {
            new Auction
            {
                Auction_Id = Guid.NewGuid().ToString(),
                Title = "Cintamani Stone",
                Description = "A fabled gemstone believed to grant wishes, from Shambhala.",
                Starting_Price = 1000,
                Current_Price = 1000,
                Created_At = DateTime.UtcNow.AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss"),
                End_Date = DateTime.UtcNow.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss"),
            },
            new Auction
            {
                Auction_Id = Guid.NewGuid().ToString(),
                Title = "Avery's Cross",
                Description = "A golden cross from the treasure of the infamous pirate Henry Avery.",
                Starting_Price = 2000,
                Current_Price = 2000,
                Created_At = DateTime.UtcNow.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss"),
                End_Date = DateTime.UtcNow.AddHours(11).ToString("yyyy-MM-dd HH:mm:ss"),
            },
            new Auction
            {
                Auction_Id = Guid.NewGuid().ToString(),
                Title = "Sir Francis Drake's Ring",
                Description = "A ring belonging to the legendary explorer Sir Francis Drake.",
                Starting_Price = 1500,
                Current_Price = 1500,
                Created_At = DateTime.UtcNow.AddSeconds(3).ToString("yyyy-MM-dd HH:mm:ss"),
                End_Date = DateTime.UtcNow.AddHours(12).ToString("yyyy-MM-dd HH:mm:ss"),
            },
            new Auction
            {
                Auction_Id = Guid.NewGuid().ToString(),
                Title = "Sheba's Crown",
                Description = "A crown belonging to the Queen of Sheba, adorned with precious gems.",
                Starting_Price = 3000,
                Current_Price = 3000,
                Created_At = DateTime.UtcNow.AddSeconds(4).ToString("yyyy-MM-dd HH:mm:ss"),
                End_Date = DateTime.UtcNow.AddHours(13).ToString("yyyy-MM-dd HH:mm:ss"),
            },
            new Auction
            {
                Auction_Id = Guid.NewGuid().ToString(),
                Title = "Fleming's Maps",
                Description = "A collection of maps created by the explorer Robert Fleming.",
                Starting_Price = 1800,
                Current_Price = 1800,
                Created_At = DateTime.UtcNow.AddSeconds(5).ToString("yyyy-MM-dd HH:mm:ss"),
                End_Date = DateTime.UtcNow.AddHours(14).ToString("yyyy-MM-dd HH:mm:ss"),
            }
        };

        foreach (var auction in auctions)
        {
            await auctionService.InsertAuctionAsync(auction);
            await InsertBidsForAuction(auctionService, auction.Auction_Id);
            await InsertMessagesForAuction(chatService, auction.Auction_Id);
        }
    }

    private static async Task InsertBidsForAuction(IAuctionService auctionService, string auctionId)
    {
        var bidderNames = new[]
        {
            "Nathan Drake", "Victor Sullivan", "Elena Fisher", "Chloe Frazer",
            "Samuel Drake", "Charlie Cutter", "Rafe Adler", "Nadine Ross",
            "Eddy Raja", "Harry Flynn", "Zoran Lazarevic", "Marisa Chase"
        };

        var random = new Random();
        for (int i = 1; i <= 6; i++)
        {
            var auctionBid = new Auction_Bid
            {
                Bid_Id = Guid.NewGuid().ToString(),
                Auction_Id = auctionId,
                Username = bidderNames[random.Next(bidderNames.Length)],
                Bid_Amount = i * 5000,
                Timestamp = DateTime.UtcNow.AddSeconds(+i).ToString("yyyy-MM-dd HH:mm:ss")
            };
            await auctionService.InsertBidAsync(auctionBid);
        }
    }

    private static async Task InsertMessagesForAuction(IChatService chatService, string auctionId)
    {
        var messageContents = new[]
        {
            "I've been looking for this artifact for years!",
            "This piece would be perfect for my collection.",
            "I can't let this one slip away.",
            "I've seen things like this in my adventures.",
            "This has to be worth more than the starting bid.",
            "Do you think it's really authentic?",
            "The history behind this is incredible!",
            "I wonder what secrets it holds."
        };

        var usernames = new[]
        {
            "Nathan Drake", "Victor Sullivan", "Elena Fisher", "Chloe Frazer",
            "Samuel Drake", "Charlie Cutter", "Rafe Adler", "Nadine Ross",
            "Eddy Raja", "Harry Flynn", "Zoran Lazarevic", "Marisa Chase"
        };

        var random = new Random();
        for (int i = 1; i <= 8; i++)
        {
            var chatMessage = new Chat_Message
            {
                Message_Id = Guid.NewGuid().ToString(),
                Auction_Id = auctionId,
                Username = usernames[random.Next(usernames.Length)],
                Message_Text = messageContents[random.Next(messageContents.Length)],
                Created_Timestamp = DateTime.UtcNow.AddSeconds(+i).ToString("yyyy-MM-dd HH:mm:ss"),
            };
            await chatService.InsertMessageAsync(chatMessage);
        }
    }
}
