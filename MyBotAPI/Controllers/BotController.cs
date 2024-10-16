using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Schema;
using Microsoft.Identity.Abstractions;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MyBotAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/messages")]
    public class BotController(ILogger<BotController> logger, IAuthorizationHeaderProvider auth) : ControllerBase //, IAuthorizationHeaderProvider auth
    {
        [HttpPost]
        public async Task PostAsync(object body, CancellationToken cancellationToken)
        {
            logger.LogInformation("Received a message from the Bot Framework");
            var header = Request.Headers["Authorization"].ToString();
            var token1 = header.Substring("Bearer:".Length);
            logger.LogInformation($"{token1}");

            logger.LogInformation(System.Text.Json.JsonSerializer.Serialize(body));

            Activity  act = JsonConvert.DeserializeObject<Activity>(body.ToString()!)!;


            HttpClient client = new HttpClient();

            string[] Scopes = ["https://api.botframework.com/.default"];


            var tok = await auth.CreateAuthorizationHeaderForAppAsync(Scopes[0]);

            var token = tok.Substring("Bearer ".Length);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string url = $"{act.ServiceUrl}/v3/conversations/{act.Conversation.Id}/activities";

            var respMsg = new Activity
            {
                ChannelId = act.ChannelId,
                Conversation = new ConversationAccount
                {
                    Id = act.Conversation.Id,
                },
                From = new ChannelAccount
                {
                    Id = act.Recipient.Id,
                    Name = act.Recipient.Name,
                },
                InputHint = "acceptingInput",
                Recipient = new ChannelAccount
                {
                    Id = act.From.Id
                },
                Text = "Hello from the bot " + DateTime.Now.ToString("o"),
                Type = "message"
            };

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Content = new StringContent(JsonConvert.SerializeObject(respMsg), System.Text.Encoding.UTF8, "application/json");

            var resp = await client.SendAsync(httpRequest);
            logger.LogInformation($"Response: {resp.StatusCode}");
        }
    }
}
