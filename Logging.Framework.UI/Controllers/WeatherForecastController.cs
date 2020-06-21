using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Global.Clients;

namespace Logging.Framework.UI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
      
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IMovieDetailsClient _movieDetailsClient;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMovieDetailsClient movieDetailsClient)
        {
            _logger = logger;
           _movieDetailsClient = movieDetailsClient;  
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            _logger.LogWarning("Error may occur after this");
            _logger.LogInformation("Some info");
            _logger.LogError("Error");
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost]
        [Route("Submit/{title}")]
        public async Task<IActionResult> Submit(string title)
        {
            var movieDetail = await _movieDetailsClient.GetMovieDetailsAsync(title);
            return Ok(movieDetail);
        }

    }
}
