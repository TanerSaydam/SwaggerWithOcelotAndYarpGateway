using Microsoft.AspNetCore.Mvc;

namespace SwaggerOcelot.Order.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("Hello world");
    }
}
