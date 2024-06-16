using KafkaAuction.Dtos;
using KafkaAuction.Models;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;

namespace KafkaAuction.Services.Interfaces;

public interface IUserLocationService
{
    Task<TablesResponse[]> CreateUserLocationTableAsync(CancellationToken cancellationToken = default);
    Task<(HttpResponseMessage, UserLocationDto)> InsertOrUpdateUserLocationAsync(User_Location userLocation);
    Task<List<DropResourceResponseDto>> DropTablesAsync();
    Task<List<string>> GetUsersOnPage(string page);
    Task<List<User_Location>?> GetAllUserLocations();
    Task<List<string>> GetPagesForUser(string userLocationId);
}