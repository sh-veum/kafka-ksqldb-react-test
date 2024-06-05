using KafkaAuction.Dtos;
using KafkaAuction.Models;
using KafkaAuction.Services.Interfaces;
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
        var userLocation = new User_Location
        {
            User_Location_Id = Guid.NewGuid().ToString(),
            User_Id = userLocationDto.User_Id,
            Pages = userLocationDto.Pages
        };

        HttpResponseMessage result = await _userLocationService.InsertUserLocationAsync(userLocation);

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
    [ProducesResponseType(typeof(User_Location), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserLocation([FromQuery] string page)
    {
        var result = await _userLocationService.GetUsersOnPage(page);

        return Ok(result);
    }
}