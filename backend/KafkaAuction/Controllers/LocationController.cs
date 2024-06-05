using System.Security.Cryptography;
using System.Text;
using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Statements;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/location")]
public class LocationController : ControllerBase
{
    private readonly ILogger<LocationController> _logger;
    private readonly IUserLocationService _userLocationService;

    public LocationController(ILogger<LocationController> logger, IUserLocationService userLocationService)
    {
        _logger = logger;
        _userLocationService = userLocationService;
    }


    [HttpGet("create_location_table")]
    public async Task<IActionResult> CreateLocationTable()
    {
        var result = await _userLocationService.CreateUserLocationTableAsync();

        return Ok(result);
    }

    [HttpPost("insert_location")]
    public async Task<IActionResult> InsertLocation([FromBody] UserLocationDto userLocationDto)
    {
        var userLocationId = GetSessionUserLocationId();
        _logger.LogInformation("User Location Id: {UserLocationId}", userLocationId);
        var userLocation = new User_Location
        {
            User_Location_Id = userLocationId,
            User_Id = userLocationDto.User_Id,
            Pages = userLocationDto.Pages
        };

        HttpResponseMessage result = await _userLocationService.InsertOrUpdateUserLocationAsync(userLocation);

        if (!result.IsSuccessStatusCode)
        {
            return BadRequest(result.ReasonPhrase);
        }
        else
        {
            return Ok(userLocation);
        }
    }

    [HttpPost("remove_location")]
    public async Task<IActionResult> RemoveLocation([FromBody] UserLocationRemoveDto userLocationDto)
    {
        string userLocationId;

        if (string.IsNullOrEmpty(userLocationDto.User_Location_Id))
        {
            userLocationId = GetSessionUserLocationId();
        }
        else
        {
            userLocationId = userLocationDto.User_Location_Id;
        }

        var userPages = await _userLocationService.GetPagesForUser(userLocationId);

        _logger.LogInformation("User Pages before : {UserPages}", userPages);

        if (userPages != null && userPages.Contains(userLocationDto.Page))
        {
            userPages = userPages.Where(p => p != userLocationDto.Page).ToList();
        }

        _logger.LogInformation("User Pages after : {UserPages}", userPages);

        _logger.LogInformation("User Location Id: {UserLocationId}", userLocationId);
        var userLocation = new User_Location
        {
            User_Location_Id = userLocationId,
            User_Id = userLocationDto.User_Id,
            Pages = userPages?.ToArray() ?? []
        };

        HttpResponseMessage result = await _userLocationService.InsertOrUpdateUserLocationAsync(userLocation);

        if (!result.IsSuccessStatusCode)
        {
            return BadRequest(result.ReasonPhrase);
        }
        else
        {
            return Ok(userLocation);
        }
    }

    [HttpPost("drop_location_table")]
    public async Task<IActionResult> DropLocationTable()
    {
        await _userLocationService.DropTablesAsync();

        return Ok();
    }

    [HttpGet("get_all_locations")]
    [ProducesResponseType(typeof(User_Location), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllLocations()
    {
        var result = await _userLocationService.GetAllUserLocations();

        return Ok(result);
    }

    [HttpGet("get_users_on_page")]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUsersOnPage([FromQuery] string page)
    {
        var result = await _userLocationService.GetUsersOnPage(page);

        return Ok(result);
    }

    [HttpGet("get_pages_for_user")]
    [ProducesResponseType(typeof(string[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPagesForUser([FromQuery] string? userLocationId)
    {
        if (string.IsNullOrEmpty(userLocationId))
        {
            var userSessionLocationId = GetSessionUserLocationId();
            _logger.LogInformation("User Location Id: {UserLocationId}", userLocationId);
            var result = await _userLocationService.GetPagesForUser(userSessionLocationId);
            return Ok(result);
        }
        else
        {
            var result = await _userLocationService.GetPagesForUser(userLocationId);
            return Ok(result);
        }

    }

    private string GetSessionUserLocationId()
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        _logger.LogInformation("IP Address: {IpAddress}", ipAddress);
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        _logger.LogInformation("User Agent: {UserAgent}", userAgent);

        var combinedString = $"{ipAddress}-{userAgent}";
        _logger.LogInformation("Combined String: {CombinedString}", combinedString);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(combinedString));
        return new Guid(hash.Take(16).ToArray()).ToString();
    }
}