using System.Security.Cryptography;
using System.Text;
using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
using ksqlDB.RestApi.Client.KSql.RestApi.Responses.Tables;
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


    [HttpPost("create_location_table")]
    [ProducesResponseType(typeof(TablesResponse[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateLocationTable()
    {
        var result = await _userLocationService.CreateUserLocationTableAsync();

        return Ok(result);
    }

    [HttpDelete("drop_location_table")]
    [ProducesResponseType(typeof(DropResourceResponseDto[]), StatusCodes.Status200OK)]
    public async Task<IActionResult> DropLocationTable()
    {
        var result = await _userLocationService.DropTablesAsync();

        return Ok(result);
    }

    [HttpPost("insert_locations")]
    [ProducesResponseType(typeof(UserLocationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> InsertLocations([FromBody] UserLocationDto userLocationDto)
    {
        var userLocationId = GetSessionUserLocationId(userLocationDto.User_Id);
        _logger.LogInformation("User Location Id: {UserLocationId}", userLocationId);
        var userLocation = new User_Location
        {
            User_Location_Id = userLocationId,
            User_Id = userLocationDto.User_Id,
            Pages = userLocationDto.Pages
        };

        var (httpResponseMessage, userLocationResult) = await _userLocationService.InsertOrUpdateUserLocationAsync(userLocation);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return BadRequest(httpResponseMessage.ReasonPhrase);
        }
        else
        {
            return Ok(userLocationResult);
        }
    }


    [HttpPost("add_location")]
    [ProducesResponseType(typeof(UserLocationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> AddLocation([FromBody] UserLocationUpdateDto userLocationDto)
    {
        string userLocationId;

        if (string.IsNullOrEmpty(userLocationDto.User_Location_Id))
        {
            userLocationId = GetSessionUserLocationId(userLocationDto.User_Id);
        }
        else
        {
            userLocationId = userLocationDto.User_Location_Id;
        }

        var userPages = await _userLocationService.GetPagesForUser(userLocationId) ?? [];

        if (!userPages.Contains(userLocationDto.Page))
        {
            userPages.Add(userLocationDto.Page);
        }

        var userLocation = new User_Location
        {
            User_Location_Id = userLocationId,
            User_Id = userLocationDto.User_Id,
            Pages = [.. userPages]
        };

        var (httpResponseMessage, userLocationResult) = await _userLocationService.InsertOrUpdateUserLocationAsync(userLocation);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return BadRequest(httpResponseMessage.ReasonPhrase);
        }
        else
        {
            return Ok(userLocationResult);
        }
    }

    [HttpDelete("remove_location")]
    [ProducesResponseType(typeof(UserLocationDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveLocation([FromBody] UserLocationUpdateDto userLocationDto)
    {
        string userLocationId;

        if (string.IsNullOrEmpty(userLocationDto.User_Location_Id))
        {
            userLocationId = GetSessionUserLocationId(userLocationDto.User_Id);
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

        if (userPages == null || userPages.Count == 0)
        {
            userPages = ["none"];
        }

        _logger.LogInformation("User Pages after : {UserPages}", userPages);

        _logger.LogInformation("User Location Id: {UserLocationId}", userLocationId);
        var userLocation = new User_Location
        {
            User_Location_Id = userLocationId,
            User_Id = userLocationDto.User_Id,
            Pages = userPages?.ToArray() ?? ["none"]
        };

        var (httpResponseMessage, userLocationResult) = await _userLocationService.InsertOrUpdateUserLocationAsync(userLocation);

        if (!httpResponseMessage.IsSuccessStatusCode)
        {
            return BadRequest(httpResponseMessage.ReasonPhrase);
        }
        else
        {
            return Ok(userLocationResult);
        }
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
    public async Task<IActionResult> GetPagesForUser([FromQuery] string? userLocationId, [FromQuery] string userId = "anon")
    {
        if (string.IsNullOrEmpty(userLocationId))
        {
            var userSessionLocationId = GetSessionUserLocationId(userId);
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

    private string GetSessionUserLocationId(string userId = "anon")
    {
        var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        _logger.LogInformation("IP Address: {IpAddress}", ipAddress);
        var userAgent = HttpContext.Request.Headers.UserAgent.ToString();
        _logger.LogInformation("User Agent: {UserAgent}", userAgent);

        var combinedString = userId != "anon"
            ? $"{ipAddress}-{userAgent}-{userId}"
            : $"{ipAddress}-{userAgent}";

        _logger.LogInformation("Combined String: {CombinedString}", combinedString);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(combinedString));
        return new Guid(hash.Take(16).ToArray()).ToString();
    }
}