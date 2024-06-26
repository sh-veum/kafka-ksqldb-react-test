using KafkaAuction.Constants;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using KafkaAuction.Utilities;

namespace KafkaAuction.Services;

public class AuctionStatusChecker : BackgroundService
{
    private readonly ILogger<AuctionStatusChecker> _logger;
    private readonly IAuctionService _auctionService;
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly int delayInMinutes = 1;

    public AuctionStatusChecker(ILogger<AuctionStatusChecker> logger, IAuctionService auctionService, IKSqlDbRestApiProvider restApiProvider)
    {
        _logger = logger;
        _auctionService = auctionService;
        _restApiProvider = restApiProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Auction status checker starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndCloseEndedAuctions(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking and closing ended auctions");
            }

            await Task.Delay(TimeSpan.FromMinutes(delayInMinutes), stoppingToken);
        }

        _logger.LogInformation("Auction status checker stopping.");
    }

    private async Task CheckAndCloseEndedAuctions(CancellationToken cancellationToken)
    {
        var auctionDtos = await _auctionService.GetAllAuctions();
        var now = DateTime.UtcNow;

        foreach (var auctionDto in auctionDtos.Where(a => a.Is_Open && DateTime.Parse(a.End_Date) < now))
        {
            _logger.LogInformation("Closing auction {AuctionId} as it has ended", auctionDto.Auction_Id);

            auctionDto.Is_Open = false;

            // Optionally set the winner if you want to automatically determine it
            var bids = await _auctionService.GetBidsForAuction(auctionDto.Auction_Id);
            auctionDto.Winner = bids.Count == 0 ? "No bids" : bids.OrderByDescending(b => b.Bid_Amount).First().Username;

            var auction = new Auction
            {
                Auction_Id = auctionDto.Auction_Id,
                Title = auctionDto.Title,
                Description = auctionDto.Description,
                Starting_Price = auctionDto.Starting_Price,
                Current_Price = auctionDto.Current_Price ?? 0,
                Leader = auctionDto.Leader ?? "None",
                Number_Of_Bids = auctionDto.Number_Of_Bids ?? 0,
                Winner = auctionDto.Winner,
                Created_At = auctionDto.Created_At,
                End_Date = auctionDto.End_Date,
                Is_Open = auctionDto.Is_Open
            };


            var inserter = new EntityInserter<Auction>(_restApiProvider, _logger);
            await inserter.InsertAsync(TableNameConstants.Auctions, auction);
        }
    }
}