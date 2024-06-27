using KafkaAuction.Services.Interfaces;

namespace KafkaAuction.Services;

public class ServiceFactory : IServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IAuctionService CreateAuctionService()
    {
        return _serviceProvider.GetRequiredService<IAuctionService>();
    }

    public IAuctionBidService CreateAuctionBidService()
    {
        return _serviceProvider.GetRequiredService<IAuctionBidService>();
    }

    public IAuctionWithBidsService CreateAuctionWithBidsService()
    {
        return _serviceProvider.GetRequiredService<IAuctionWithBidsService>();
    }
}
