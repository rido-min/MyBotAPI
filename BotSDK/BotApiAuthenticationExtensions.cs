using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.JsonWebTokens;

namespace BotSDK
{
    public static class BotApiAuthenticationExtensions
    {
        static string[] validTokenIssuers = [ "https://api.botframework.com",
                                              "https://sts.windows.net/d6d49420-f39b-4df7-a1dc-d59a935871db/",
                                              "https://login.microsoftonline.com/d6d49420-f39b-4df7-a1dc-d59a935871db/v2.0",
                                              "https://sts.windows.net/f8cdef31-a31e-4b4a-93e4-5f571e91255a/",
                                              "https://login.microsoftonline.com/f8cdef31-a31e-4b4a-93e4-5f571e91255a/v2.0" ];

        public static void AddBotApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddMicrosoftIdentityWebApiAuthentication(configuration)
                .EnableTokenAcquisitionToCallDownstreamApi()
                .AddInMemoryTokenCaches();

            services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidIssuers = validTokenIssuers,
                    ValidAudience = configuration["AzureAd:ClientId"],
                    SignatureValidator = (token, parameters) => new JsonWebToken(token),
                };
            });
        }
    }
}
