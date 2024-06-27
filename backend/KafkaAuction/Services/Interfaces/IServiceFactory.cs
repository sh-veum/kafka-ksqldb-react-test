namespace KafkaAuction.Services.Interfaces;

public interface IServiceFactory
{
    IAuctionService CreateAuctionService();
    IAuctionBidService CreateAuctionBidService();
    IAuctionWithBidsService CreateAuctionWithBidsService();
}