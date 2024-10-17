using BotSDK;
using MyBotAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddBotApiAuthentication(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddTransient<BotResponseService>();

var app = builder.Build();

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
