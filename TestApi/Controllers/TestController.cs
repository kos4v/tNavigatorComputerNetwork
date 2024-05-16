using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace TestApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController(ILogger<TestController> logger) : ControllerBase
    {
        [HttpPatch]
        public IActionResult Patch(
            [FromQuery] string secret,
            string calculationStatus,
            [FromBody] tNavigatorModels.Result.ModelResult? modelResult)
        {
            var validSecret = "asd";
            if (validSecret != secret)
                return Conflict("asd");

            Debug.WriteLine(calculationStatus);
            Debug.WriteLine(JsonSerializer.Serialize(modelResult));
            return Ok();
        }

        [HttpPost]
        public IActionResult Post(
            [FromQuery] string secret,
            [FromForm] IFormFile[] files)
        {
            var validSecret = "asd";
            if (validSecret != secret)
                return Conflict("asd");

            foreach (IFormFile file in files)
            {
                Debug.WriteLine(file.FileName);
            }

            return Ok();
        }
    }
}