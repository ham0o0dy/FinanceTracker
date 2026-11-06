using FinanceTracker.BusinessLogic.DTOs.Users;
using FinanceTracker.BusinessLogic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Controllers;

[Route("api/users")]
[Authorize]
public class UsersController(IUserService userService) : ApiControllerBase
{
    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> GetMe()
    {
        return Ok(await userService.GetMeAsync(CurrentUserId));
    }

    [HttpPut("me")]
    public async Task<ActionResult<UserDto>> UpdateMe(UpdateUserRequest request)
    {
        return Ok(await userService.UpdateMeAsync(CurrentUserId, request));
    }
}
