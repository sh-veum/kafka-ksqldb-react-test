using System.ComponentModel.DataAnnotations;

namespace KafkaAuction.Dtos;

public record UserLocationDto
{
    public required string User_Id { get; set; }
    public required string[] Pages { get; set; }
}
