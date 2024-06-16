using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Streams;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;
using Microsoft.AspNetCore.Mvc;

namespace KafkaAuction.Controllers;

[ApiController]
[Route("api/overview")]
public class OverviewController : ControllerBase
{
    private readonly ILogger<OverviewController> _logger;
    private readonly IKsqlDbService _ksqlDbService;

    public OverviewController(ILogger<OverviewController> logger, IKsqlDbService ksqlDbService)
    {
        _logger = logger;
        _ksqlDbService = ksqlDbService;
    }

    [HttpGet("check_tables")]
    [ProducesResponseType(typeof(TablesResponse[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckTables()
    {
        var result = await _ksqlDbService.CheckTablesAsync();

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpGet("check_streams")]
    [ProducesResponseType(typeof(StreamsResponse[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> CheckStreams()
    {
        var result = await _ksqlDbService.CheckStreamsAsync();

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(result);
    }

    [HttpDelete("drop_table")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> DropTable([FromQuery] string tableName)
    {
        var result = await _ksqlDbService.DropSingleTablesAsync(tableName);

        if (!result.IsSuccessStatusCode)
        {
            return BadRequest();
        }
        else
        {
            return Ok(result.Content.ReadAsStringAsync().Result);
        }
    }

    [HttpDelete("drop_stream")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public async Task<IActionResult> DropStream([FromQuery] string streamName)
    {
        var result = await _ksqlDbService.DropSingleStreamAsync(streamName);

        if (!result.IsSuccessStatusCode)
        {
            return BadRequest();
        }
        else
        {
            return Ok(result.Content.ReadAsStringAsync().Result);
        }
    }
}