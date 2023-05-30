using food_delivery.Models;
using food_delivery.Services;
using Microsoft.AspNetCore.Mvc;

namespace food_delivery.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{

    private readonly ILogger<MerchantsController> _logger;

    public HealthController(ILogger<MerchantsController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IActionResult Get()
    {
        return Ok("welcome to customers-backend api");
    }

}
