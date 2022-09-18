using API;
using API.Controllers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddChuckAuthentication();

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.RegisterDi();
builder.ConfigureLogger();
builder.ConfigureSwagger();

var app = builder.Build();

app.AddUserEndpoints();
app.AddQuoteEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Make the implicit Program class public so test projects can access it
namespace API
{
    public partial class Program { }
}