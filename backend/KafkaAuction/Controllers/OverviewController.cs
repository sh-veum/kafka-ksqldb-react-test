using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.Query.Context;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

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
    public async Task<IActionResult> DropTable([FromQuery] string tableName)
    {
        var result = await _ksqlDbService.DropSingleTablesAsync(tableName);

        if (result == null)
        {
            return BadRequest();
        }
        else
        {
            return Ok(JToken.Parse(result).ToString(Newtonsoft.Json.Formatting.Indented));
        }
    }

    [HttpDelete("drop_stream")]
    public async Task<IActionResult> DropStream([FromQuery] string streamName)
    {
        var result = await _ksqlDbService.DropSingleStreamAsync(streamName);

        if (result == null)
        {
            return BadRequest();
        }
        else
        {
            return Ok(JToken.Parse(result).ToString(Newtonsoft.Json.Formatting.Indented));
        }
    }
}