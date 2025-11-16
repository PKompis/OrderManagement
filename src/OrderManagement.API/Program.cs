using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OrderManagement.API.Extensions;
using OrderManagement.API.Mappings;
using OrderManagement.API.Options;
using OrderManagement.API.Security;
using OrderManagement.Application;
using OrderManagement.Application.Common.Abstractions;
using OrderManagement.Infrastructure;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, config) =>
{
    config
        .ReadFrom.Configuration(ctx.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console(); //Dummy logging
});

var jwtOptions = builder.Configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SecretKey));

builder.Services
    .AddControllers().Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddAutoMapper(cfg =>
    {
        cfg.AddProfile<OrdersApiProfile>();
        cfg.AddProfile<MenuApiProfile>();
        cfg.AddProfile<AdminApiProfile>();
    })
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AddOptions<JwtOptions>()
        .Bind(builder.Configuration.GetSection(JwtOptions.SectionName))
        .Validate(o => !string.IsNullOrWhiteSpace(o.SecretKey), "Jwt:SecretKey is required")
        .ValidateOnStart()
.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    })
.Services
    .AddAuthorization()
    .AddHttpContextAccessor()
    .AddScoped<ICurrentUser, CurrentUser>()
    .AddScoped<ITokenService, JwtTokenService>()
    .AddGlobalRateLimiter();

var app = builder.Build();

//using (var scope = app.Services.CreateScope())
//{
//    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    await db.Database.MigrateAsync();
//}

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