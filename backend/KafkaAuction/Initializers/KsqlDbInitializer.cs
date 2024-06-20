using System.Diagnostics;
using KafkaAuction.Constants;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;
using Stream = ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams.Stream;

namespace KafkaAuction.Initializers;

public class AuctionConfig
{
    public int AuctionCount { get; set; } = 20;
    public int BidsPerAuction { get; set; } = 30;
    public int MessagesPerAuction { get; set; } = 50;
}

public static class KsqlDbInitializer
{
    public static async Task InitializeAsync(IAuctionService auctionService, IChatService chatService, IUserLocationService userLocationService, IKsqlDbService ksqlDbService, ILogger logger, AuctionConfig config)
    {
        TablesResponse[] tablesResponse = await ksqlDbService.CheckTablesAsync();
        StreamsResponse[] streamsResponse = await ksqlDbService.CheckStreamsAsync();

        var tables = tablesResponse[0]?.Tables ?? [];
        var streams = streamsResponse[0]?.Streams ?? [];

        logger.LogInformation("Existing Tables: " + string.Join(", ", tables.Select(t => t.Name)));
        logger.LogInformation("Existing Streams: " + string.Join(", ", streams.Select(s => s.Name)));

        var createdAny = await EnsureTablesAndStreamsExist(auctionService, chatService, userLocationService, tables, streams, logger);

        if (createdAny)
        {
            logger.LogInformation("Waiting 5 seconds to allow ksqlDB to process the new tables and streams.");
            Thread.Sleep(5000);
        }

        var auctions = await auctionService.GetAllAuctions();
        if (auctions.Count == 0)
        {
            var stopwatch = Stopwatch.StartNew();
            await InsertAuctionsAndBidsAndMessages(auctionService, chatService, config);
            stopwatch.Stop();
            logger.LogInformation("Initialized the database in {0} seconds.", stopwatch.Elapsed.TotalSeconds);
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

    private static async Task InsertAuctionsAndBidsAndMessages(IAuctionService auctionService, IChatService chatService, AuctionConfig config)
    {
        var predefinedAuctions = new List<Auction>
            {
                new() {
                    Auction_Id = Guid.NewGuid().ToString(),
                    Title = "Cintamani Stone",
                    Description = "A fabled gemstone believed to grant wishes, from Shambhala.",
                    Starting_Price = 1000,
                    Current_Price = 1000,
                    Created_At = DateTime.UtcNow.AddSeconds(1).ToString("yyyy-MM-dd HH:mm:ss"),
                    End_Date = DateTime.UtcNow.AddHours(10).ToString("yyyy-MM-dd HH:mm:ss"),
                },
                new() {
                    Auction_Id = Guid.NewGuid().ToString(),
                    Title = "Avery's Cross",
                    Description = "A golden cross from the treasure of the infamous pirate Henry Avery.",
                    Starting_Price = 2000,
                    Current_Price = 2000,
                    Created_At = DateTime.UtcNow.AddSeconds(2).ToString("yyyy-MM-dd HH:mm:ss"),
                    End_Date = DateTime.UtcNow.AddHours(11).ToString("yyyy-MM-dd HH:mm:ss"),
                },
                new() {
                    Auction_Id = Guid.NewGuid().ToString(),
                    Title = "Sir Francis Drake's Ring",
                    Description = "A ring belonging to the legendary explorer Sir Francis Drake.",
                    Starting_Price = 1500,
                    Current_Price = 1500,
                    Created_At = DateTime.UtcNow.AddSeconds(3).ToString("yyyy-MM-dd HH:mm:ss"),
                    End_Date = DateTime.UtcNow.AddHours(12).ToString("yyyy-MM-dd HH:mm:ss"),
                },
                new() {
                    Auction_Id = Guid.NewGuid().ToString(),
                    Title = "Sheba's Crown",
                    Description = "A crown belonging to the Queen of Sheba, adorned with precious gems.",
                    Starting_Price = 3000,
                    Current_Price = 3000,
                    Created_At = DateTime.UtcNow.AddSeconds(4).ToString("yyyy-MM-dd HH:mm:ss"),
                    End_Date = DateTime.UtcNow.AddHours(13).ToString("yyyy-MM-dd HH:mm:ss"),
                },
                new() {
                    Auction_Id = Guid.NewGuid().ToString(),
                    Title = "Thievius Raccoonus",
                    Description = "A book detailing the Cooper Clan's thieving techniques and history",
                    Starting_Price = 1800,
                    Current_Price = 1800,
                    Created_At = DateTime.UtcNow.AddSeconds(5).ToString("yyyy-MM-dd HH:mm:ss"),
                    End_Date = DateTime.UtcNow.AddHours(14).ToString("yyyy-MM-dd HH:mm:ss"),
                }
            };

        for (int i = 0; i < config.AuctionCount; i++)
        {
            var auction = predefinedAuctions[i % predefinedAuctions.Count];
            auction.Auction_Id = Guid.NewGuid().ToString();
            auction.Created_At = DateTime.UtcNow.AddSeconds(i + 1).ToString("yyyy-MM-dd HH:mm:ss");
            auction.End_Date = DateTime.UtcNow.AddHours(10 + i).ToString("yyyy-MM-dd HH:mm:ss");

            await auctionService.InsertAuctionAsync(auction);
            await InsertBidsForAuction(auctionService, auction.Auction_Id, config.BidsPerAuction);
            await InsertMessagesForAuction(chatService, auction.Auction_Id, config.MessagesPerAuction);
        }
    }

    private static async Task InsertBidsForAuction(IAuctionService auctionService, string auctionId, int bidCount)
    {
        var bidderNames = BidderNames.GetBidderNames();
        var random = new Random();
        for (int i = 1; i <= bidCount; i++)
        {
            var auctionBid = new Auction_Bid
            {
                Bid_Id = Guid.NewGuid().ToString(),
                Auction_Id = auctionId,
                Username = bidderNames[random.Next(bidderNames.Length)],
                Bid_Amount = i * 5000,
                Timestamp = DateTime.UtcNow.AddSeconds(i).ToString("yyyy-MM-dd HH:mm:ss")
            };
            await auctionService.InsertBidAsync(auctionBid);
        }
    }

    private static async Task InsertMessagesForAuction(IChatService chatService, string auctionId, int messageCount)
    {
        var characterSentences = CharacterSentences.GetCharacterSentences();
        var random = new Random();
        for (int i = 1; i <= messageCount; i++)
        {
            var username = characterSentences.Keys.ElementAt(random.Next(characterSentences.Count));
            var messageContents = characterSentences[username];
            var chatMessage = new Chat_Message
            {
                Message_Id = Guid.NewGuid().ToString(),
                Auction_Id = auctionId,
                Username = username,
                Message_Text = messageContents[random.Next(messageContents.Length)],
                Created_Timestamp = DateTime.UtcNow.AddSeconds(i).ToString("yyyy-MM-dd HH:mm:ss"),
            };
            await chatService.InsertMessageAsync(chatMessage);
        }
    }
}

public static class BidderNames
{
    public static string[] GetBidderNames()
    {
        return new[]
        {
                "Nathan Drake", "Victor Sullivan", "Elena Fisher", "Chloe Frazer",
                "Samuel Drake", "Charlie Cutter", "Rafe Adler", "Nadine Ross",
                "Eddy Raja", "Harry Flynn", "Zoran Lazarevic", "Marisa Chase",
                "Sly Cooper", "Bentley", "Murray", "Carmelita Fox"
            };
    }
}

public static class CharacterSentences
{
    private static readonly Dictionary<string, string[]> characterSentences = new()
    {
        { "Nathan Drake", new[] { "I've been looking for this artifact for years!", "I've seen things like this in my adventures.", "The history behind this is incredible!" } },
        { "Victor Sullivan", new[] { "This piece would be perfect for my collection.", "Do you think it's really authentic?", "This is exactly what I've been after." } },
        { "Elena Fisher", new[] { "I can't let this one slip away.", "This has to be worth more than the starting bid.", "I need to capture this on film!" } },
        { "Chloe Frazer", new[] { "I've seen things like this in my adventures.", "I wonder what secrets it holds.", "This would be quite a steal." } },
        { "Samuel Drake", new[] { "I've been looking for this artifact for years!", "I can't wait to see who wins this auction.", "This is just the beginning of a great adventure." } },
        { "Charlie Cutter", new[] { "Do you think it's really authentic?", "I've heard stories about this artifact.", "Let's get our hands on this beauty." } },
        { "Rafe Adler", new[] { "I can't let this one slip away.", "I think we should bid on this one, it could be valuable.", "I will own this artifact, no matter the cost." } },
        { "Nadine Ross", new[] { "This has to be worth more than the starting bid.", "I wonder what secrets it holds.", "We must secure this, no matter the competition." } },
        { "Eddy Raja", new[] { "I've been looking for this artifact for years!", "This will fetch a high price on the market.", "Let's make sure we win this auction." } },
        { "Harry Flynn", new[] { "I can't wait to see who wins this auction.", "This will be a perfect addition to my collection.", "I'm in for this artifact." } },
        { "Zoran Lazarevic", new[] { "I wonder what secrets it holds.", "This has to be worth more than the starting bid.", "This artifact will be mine." } },
        { "Marisa Chase", new[] { "I've heard stories about this artifact.", "This is a rare find.", "I think we should bid on this one, it could be valuable." } },
        { "Sly Cooper", new[] { "I've been looking for this artifact for years!", "This will be a great addition to our heists.", "I've got a plan to secure this artifact, trust me." } },
        { "Bentley", new[] { "I've analyzed the artifact, it's definitely worth the bid.", "Be careful, there might be other thieves trying to snatch this artifact.", "We'll need to outsmart everyone to win this." } },
        { "Murray", new[] { "Alright, let's make sure we crush this auction!", "We gotta outsmart those other bidders and win this thing!", "This artifact is gonna lead us to even bigger treasures, I can feel it!" } },
        { "Carmelita Fox", new[] { "I think we should bid on this one, it could be valuable.", "We need to stay focused and bid strategically.", "This is a once in a lifetime find, let's get it." } },
    };

    public static Dictionary<string, string[]> GetCharacterSentences()
    {
        return characterSentences;
    }
}

