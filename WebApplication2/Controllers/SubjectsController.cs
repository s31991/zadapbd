using Microsoft.AspNetCore.Mvc;

namespace WebApplication2.Controllers;

[ApiController]
[Route("[controller]")]
public class SubjectsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("hello world");
    }
}