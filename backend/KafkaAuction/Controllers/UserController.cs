using KafkaAuction.Dtos;
using KafkaAuction.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NetBackend.Models.Dtos;

[ApiController]
[Route("api/user")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly UserManager<UserModel> _userManager;

    public UserController(ILogger<UserController> logger, UserManager<UserModel> userManager)
    {
        _logger = logger;
        _userManager = userManager;
    }

    [HttpGet("userinfo")]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUserInfo()
    {
        try
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var userDto = new UserInfoDto
            {
                Email = user.Email,
                UserName = user.UserName,
                Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
            };

            return Ok(userDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching user info.");
            return BadRequest(ex.Message);
        }
    }

    [HttpPatch("change-username")]
    [ProducesResponseType(typeof(UserInfoDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> ChangeUsername([FromBody] UserUpdateUserNameDto userUpdateUserNameDto)
    {
        try
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            user.UserName = userUpdateUserNameDto.UserName;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                var userDto = new UserInfoDto
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    Role = (await _userManager.GetRolesAsync(user)).FirstOrDefault()
                };

                return Ok(userDto);
            }

            return BadRequest(result.Errors);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while changing username.");
            return BadRequest(ex.Message);
        }
    }
}
