using KafkaAuction.Dtos;
using KafkaAuction.Models;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;

namespace KafkaAuction.Services.Interfaces;

public interface IAuctionService
{
    Task<TablesResponse[]> CreateAuctionTableAsync(CancellationToken cancellationToken = default);
    Task<(HttpResponseMessage httpResponseMessage, AuctionDto? auctionDto)> InsertAuctionAsync(Auction auction);
    Task<List<DropResourceResponseDto>> DropAuctionTablesAsync();
    Task<List<AuctionDto>> GetAllAuctions();
    Task<List<AuctionDto>> GetAuctionsAsync(int limit);
    Task<AuctionDto?> GetAuctionDtoByIdAsync(string auction_id);
    Task<Auction?> GetAuctionByIdAsync(string auction_id);
    Task<(HttpResponseMessage httpResponseMessage, Auction? auction)> DeleteAuction(string auction_id);
    Task<(HttpResponseMessage httpResponseMessage, AuctionDto? auctionDto)> EndAuctionAsync(string auction_id);

}