using KafkaAuction.Dtos;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;

namespace KafkaAuction.Services.Interfaces;

public interface IAuctionWithBidsService
{
    Task<StreamsResponse[]> CreateAuctionsWithBidsStreamAsync(CancellationToken cancellationToken = default);
    Task<DropResourceResponseDto> DropAuctionWithBidsStreamAsync();
    Task<List<AuctionWithBidDto>> GetAllAuctionWithBidsAsync();
}
