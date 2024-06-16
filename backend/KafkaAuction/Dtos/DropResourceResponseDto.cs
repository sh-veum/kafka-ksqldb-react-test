namespace KafkaAuction.Dtos;

public class DropResourceResponseDto
{
    public required string ResourceName { get; set; }
    public bool IsSuccess { get; set; }
}
