namespace KafkaAuction.Dtos;

public record UserLocationUpdateDto
{
    public string? User_Location_Id { get; set; }
    public required string User_Id { get; set; }
    public required string Page { get; set; }
}
