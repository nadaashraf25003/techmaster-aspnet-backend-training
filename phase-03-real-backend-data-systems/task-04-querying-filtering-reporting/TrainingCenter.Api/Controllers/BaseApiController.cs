using Microsoft.AspNetCore.Mvc;

namespace TrainingCenter.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class BaseApiController : ControllerBase
{
}
