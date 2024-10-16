using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using MyBotAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddMicrosoftIdentityWebApiAuthentication(builder.Configuration)
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

string[] validTokenIssuers = [  "https://api.botframework.com",
                                "https://sts.windows.net/d6d49420-f39b-4df7-a1dc-d59a935871db/",
                                "https://login.microsoftonline.com/d6d49420-f39b-4df7-a1dc-d59a935871db/v2.0",
                                "https://sts.windows.net/f8cdef31-a31e-4b4a-93e4-5f571e91255a/",
                                "https://login.microsoftonline.com/f8cdef31-a31e-4b4a-93e4-5f571e91255a/v2.0" ];

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidIssuers = validTokenIssuers,
        ValidAudience = builder.Configuration["AzureAd:ClientId"],
        SignatureValidator = (token, parameters) => new JsonWebToken(token),
    };
});

builder.Services.AddControllers();
builder.Services.AddTransient<BotResponseService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapControllers().AllowAnonymous();
}
else
{
    app.MapControllers();
}
app.UseAuthentication();
app.UseAuthorization();
app.Run();
