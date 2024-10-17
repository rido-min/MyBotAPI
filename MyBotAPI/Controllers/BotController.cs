using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace MyBotAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/messages")]
    public class BotController(BotResponseService botResponseService) : ControllerBase
    {
        [HttpPost]
        public async Task PostAsync(object body, CancellationToken cancellationToken)
        {
            Console.WriteLine();
            Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(body));
            Console.WriteLine();

            Activity  act = JsonConvert.DeserializeObject<Activity>(body.ToString()!)!;
            await botResponseService.SendResponse("Hello from the bot", act);
        }
    }
}
