using Microsoft.Bot.Schema;
using Microsoft.Identity.Abstractions;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace MyBotAPI
{
    public class BotResponseService(IAuthorizationHeaderProvider auth,IHttpClientFactory httpClientFactory)
    {
        public async Task SendResponse(string text, Activity act)
        {
            using HttpClient client = httpClientFactory.CreateClient();

            string[] Scopes = ["https://api.botframework.com/.default"];
            var token = await auth.CreateAuthorizationHeaderForAppAsync(Scopes[0]);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Substring("Bearer ".Length));

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
                Text = text + DateTime.Now.ToString("o"),
                Type = "message"
            };

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(JsonConvert.SerializeObject(respMsg), System.Text.Encoding.UTF8, "application/json")
            };
            using var resp = await client.SendAsync(httpRequest);
            resp.EnsureSuccessStatusCode();
        }
    }
}
