using Microsoft.AspNetCore.Mvc;
using TrainingCenter.Api.DTOs.Common;

namespace TrainingCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
    protected IActionResult Success<T>(T data, string message = "Success", int statusCode = StatusCodes.Status200OK)
    {
        var response = ApiResponse<T>.SuccessResponse(data, message, statusCode);
        return StatusCode(statusCode, response);
    }

    protected IActionResult CreatedSuccess<T>(string actionName, object routeValues, T data, string message = "Resource created successfully")
    {
        var response = ApiResponse<T>.SuccessResponse(data, message, StatusCodes.Status201Created);
        return CreatedAtAction(actionName, routeValues, response);
    }
}
