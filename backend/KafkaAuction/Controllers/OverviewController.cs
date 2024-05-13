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

    [HttpGet("CheckTables")]
    public async Task<IActionResult> CheckTables()
    {
        var result = await _ksqlDbService.CheckTablesAsync();

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(JToken.Parse(result).ToString(Newtonsoft.Json.Formatting.Indented));
    }

    [HttpDelete("DropTable")]
    public async Task<IActionResult> DropTable([FromQuery] string tableName)
    {
        var result = await _ksqlDbService.DropSingleTablesAsync(tableName);

        if (result)
        {
            var currentTables = await _ksqlDbService.CheckTablesAsync();

            if (currentTables == null)
            {
                return BadRequest();
            }

            return Ok(JToken.Parse(currentTables).ToString(Newtonsoft.Json.Formatting.Indented));
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost("MakeSQLQuery")]
    public async Task<IActionResult> MakeSQLQuery([FromBody] string query)
    {
        var result = await _ksqlDbService.MakeSQLQueryAsync(query);

        if (result == null)
        {
            return BadRequest();
        }

        return Ok(JToken.Parse(result).ToString(Newtonsoft.Json.Formatting.Indented));
    }
}