using Microsoft.AspNetCore.Mvc;
using VivaApiProject.Models;

namespace VivaApiProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NumbersController : Controller
    {
        [HttpPost("second-largest")]
        public IActionResult GetSecondLargest([FromBody] RequestObj request)
        {
            //TODO: add parse try throw error
            var values = request?.RequestArrayObj?.ToList();

            if (values == null || values.Count < 2)
                return BadRequest("The array must contain at least two distinct numbers.");

            values.Sort();
            //TODO: this or [1]?
            int secondLargest = values[values.Count - 2];

            return Ok(secondLargest);
        }
    }
}
