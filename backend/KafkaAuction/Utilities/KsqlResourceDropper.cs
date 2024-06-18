using KafkaAuction.Enums;
using KafkaAuction.Services.Interfaces;

namespace KafkaAuction.Utilities;

public class KsqlResourceDropper
{
    private readonly IKSqlDbRestApiProvider _restApiProvider;
    private readonly ILogger _logger;

    public KsqlResourceDropper(IKSqlDbRestApiProvider restApiProvider, ILogger logger)
    {
        _restApiProvider = restApiProvider ?? throw new ArgumentNullException(nameof(restApiProvider));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HttpResponseMessage> DropResourceAsync(string resourceName, ResourceType resourceType)
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

        return response;
    }
}