using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Identity.Web;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddMicrosoftIdentityWebApiAuthentication(builder.Configuration, subscribeToJwtBearerMiddlewareDiagnosticsEvents: true)
    .EnableTokenAcquisitionToCallDownstreamApi(c =>
    {
        c.TenantId = builder.Configuration["AzureAd:TenantId"];
        c.ClientId = builder.Configuration["AzureAd:ClientId"];
        c.ClientSecret = "7Nv8Q~TPx3WHOr0uYO3KQN85ydFbxYkx4m4hBdze";
    })
    .AddInMemoryTokenCaches();

string[] validTokenIssuers = [
                   "https://api.botframework.com",
                    "https://sts.windows.net/d6d49420-f39b-4df7-a1dc-d59a935871db/",
                    "https://login.microsoftonline.com/d6d49420-f39b-4df7-a1dc-d59a935871db/v2.0",
                    "https://sts.windows.net/f8cdef31-a31e-4b4a-93e4-5f571e91255a/",
                    "https://login.microsoftonline.com/f8cdef31-a31e-4b4a-93e4-5f571e91255a/v2.0",
                ];
JwtSecurityTokenHandler.DefaultMapInboundClaims = false;

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options => 
{
    options.IncludeErrorDetails = true;
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5),
        ValidIssuers = validTokenIssuers,
        ValidAudience = "16766ea1-324e-443b-8dae-4b15add96a87",
        RoleClaimType = "roles",
        RequireSignedTokens = true,
        SignatureValidator = (token, parameters) => new Microsoft.IdentityModel.JsonWebTokens.JsonWebToken(token),
    };

});

builder.Services.AddControllers();

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

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.Run();
