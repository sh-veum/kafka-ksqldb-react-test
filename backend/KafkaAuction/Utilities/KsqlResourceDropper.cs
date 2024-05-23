using KafkaAuction.Enums;
using KafkaAuction.Services.Interfaces;

namespace KafkaAuction.Utilities;

public class KsqlResourceDropper
{
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly ILogger _logger;

    public KsqlResourceDropper(IKSqlDbRestApiProvider restApiProvider, ILogger logger)
    {
        _restApiProvider = restApiProvider;
        _logger = logger;
    }

    public async Task DropResourceAsync(string resourceName, ResourceType resourceType)
    {
        HttpResponseMessage response;
        if (resourceType == ResourceType.Table)
        {
            response = await _restApiProvider.DropTableAndTopic(resourceName);
        }
        else // ResourceType.Stream
        {
            response = await _restApiProvider.DropStreamAndTopic(resourceName);
        }

        if (response.IsSuccessStatusCode)
        {
            _logger.LogInformation("{ResourceType} {ResourceName} dropped successfully.", resourceType, resourceName);
        }
        else
        {
            _logger.LogError("Failed to drop {ResourceType} {ResourceName}.", resourceType, resourceName);
        }
    }
}