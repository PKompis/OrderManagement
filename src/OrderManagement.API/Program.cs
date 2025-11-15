using OrderManagement.API.Extensions;
using OrderManagement.API.Mappings;
using OrderManagement.Application;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddAutoMapper(cfg =>
    {
        cfg.AddProfile<OrdersApiProfile>();
        cfg.AddProfile<MenuApiProfile>();
        cfg.AddProfile<AdminApiProfile>();
    })
    .AddApplication();


var app = builder.Build();

app.UseExceptionHandling();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app
    .UseAuthentication()
    .UseAuthorization();

app.MapControllers();

await app.RunAsync();