namespace KafkaAuction.Services.Interfaces;

public interface IKsqlDbService
{
    Task<bool> DropSingleTablesAsync(string tableName);
    Task<string?> CheckTablesAsync();
    Task<string?> MakeSQLQueryAsync(string query);
}