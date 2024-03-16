using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController(ILogger<TestController> logger) : ControllerBase
    {
        [HttpPost]
        public IActionResult Post([FromQuery] string secret, [FromBody] tNavigatorModels.Result.ModelResult? modelResult)
        {
            var validSecret = "asd";
            if (validSecret != secret)
                return Conflict("asd");

            Debug.WriteLine(JsonSerializer.Serialize(modelResult));
            return Ok();
        }
    }
}