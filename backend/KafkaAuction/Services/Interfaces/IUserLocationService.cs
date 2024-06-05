using KafkaAuction.Models;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;

namespace KafkaAuction.Services.Interfaces;

public interface IUserLocationService
{
    Task<TablesResponse[]> CreateUserLocationTableAsync(CancellationToken cancellationToken = default);
    Task<HttpResponseMessage> InsertUserLocationAsync(User_Location userLocation);
    Task DropTablesAsync();
    Task<List<User_Location>?> GetUsersOnPage(string page);
    Task<List<User_Location>?> GetAllUserLocations();
}