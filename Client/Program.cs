// See https://aka.ms/new-console-template for more information
using Microsoft.Identity.Client;
using Microsoft.Identity.Client.Extensions.Msal;
using System.Globalization;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text.Json;

string TenantId = "69e9b82d-4842-4902-8d1e-abc5b98a55e8";
string ClientId = "e32e0313-fc8f-477b-a30d-bb0878b8901d";
string AadInstance = "https://login.microsoftonline.com/{0}/v2.0";
string Authority = string.Format(CultureInfo.InvariantCulture, AadInstance, TenantId);

string[] Scopes = { "api://e32e0313-fc8f-477b-a30d-bb0878b8901d/forecast", "api://e32e0313-fc8f-477b-a30d-bb0878b8901d/jwt" };// { "api://f5ce54cd-f7a3-47dd-b20f-ca7ded9e711c/access_as_user" };

Console.WriteLine($"Creating MSAL Client for ClientId:{ClientId} With IdentityProvider:{Authority}");

IPublicClientApplication _app = CreateMSALClientWithCache(ClientId, Authority);
var accounts = await _app.GetAccountsAsync();

Console.WriteLine($"Getting token for Scopes:{String.Join(' ', Scopes.ToList())}");
AuthenticationResult result;
if (accounts.Any())
{
    result = await _app.AcquireTokenSilent(Scopes, accounts.FirstOrDefault()).ExecuteAsync();
}
else
{
    result = await _app.AcquireTokenInteractive(Scopes).ExecuteAsync();
}

Console.WriteLine("TOKEN:");
Console.WriteLine(result.AccessToken);
Console.WriteLine();

var _httpClient = new HttpClient();
_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);


string weatherUrl = "http://localhost:5220/weatherForecast";
Console.WriteLine($"\n Weather: {weatherUrl}");
HttpResponseMessage responseWeather = await _httpClient.GetAsync(weatherUrl);
await PrintResponse(responseWeather);

string jwtUrl = "http://localhost:5220/jwtInfo";
Console.WriteLine($"\n JwtInfo: {jwtUrl}");
HttpResponseMessage jwtResponse = await _httpClient.GetAsync(jwtUrl);
await PrintResponse(jwtResponse);

Console.ReadLine();

static async Task PrintResponse(HttpResponseMessage response)
{
    if (response.IsSuccessStatusCode)
    {
        await response.Content.ReadAsStringAsync().ContinueWith(task =>
        {
            var jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = task.Result;
            var indentedJson = JsonSerializer.Serialize(JsonDocument.Parse(json).RootElement, jsonOptions);
            Console.WriteLine(indentedJson);
        });
    }
    else
    {
        Console.WriteLine("An error occurred: " + response.ReasonPhrase);
    }
}

static IPublicClientApplication CreateMSALClientWithCache(string ClientId, string Authority)
{
    var _app = PublicClientApplicationBuilder.Create(ClientId)
                   .WithAuthority(Authority)
                   .WithRedirectUri("http://localhost") // needed only for the system browser
                   .Build();
    var storageProperties =
                   new StorageCreationPropertiesBuilder(
                       "ClientConsoleApp",
                       Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                   .Build();
    var cacheHelper = MsalCacheHelper.CreateAsync(storageProperties).GetAwaiter().GetResult();
    cacheHelper.RegisterCache(_app.UserTokenCache);
    return _app;
}