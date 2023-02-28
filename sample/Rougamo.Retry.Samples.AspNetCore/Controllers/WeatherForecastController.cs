using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Rougamo.Retry.Samples.AspNetCore.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public async Task<string[]> Get()
        {
            var list = new List<string>();
            try
            {
                Failed();
                list.Add($"{nameof(Failed)} execute succeed");
            }
            catch
            {
                list.Add($"{nameof(Failed)} execute failed");
            }

            try
            {
                await FailedAsync();
                list.Add($"{nameof(FailedAsync)} execute succeed");
            }
            catch
            {
                list.Add($"{nameof(FailedAsync)} execute failed");
            }

            try
            {
                int v1, v2;
                v1 = v2 = 1;
                StaticRetrySucceed(v1, ref v2);
                list.Add($"{nameof(StaticRetrySucceed)} execute succeed");
            }
            catch
            {
                list.Add($"{nameof(StaticRetrySucceed)} execute failed");
            }

            try
            {
                await StaticRetryFailedAsync();
                list.Add($"{nameof(StaticRetryFailedAsync)} execute succeed");
            }
            catch
            {
                list.Add($"{nameof(StaticRetryFailedAsync)} execute failed");
            }

            return list.ToArray();
        }

        [RecordRetry(3)]
        private void Failed()
        {
            throw new NotImplementedException();
        }

        [RecordRetry(3)]
        private async Task FailedAsync()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }

        [RecordRetry(3)]
        private static void StaticRetrySucceed(int originValue, ref int value)
        {
            if (originValue == value)
            {
                value++;
                throw new ArgumentException();
            }
        }

        [RecordRetry(3)]
        private static async ValueTask StaticRetryFailedAsync()
        {
            await Task.Yield();
            throw new InvalidOperationException();
        }
    }
}
