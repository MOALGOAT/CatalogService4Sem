using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Driver;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using VaultSharp;
using VaultSharp.V1.AuthMethods;
using VaultSharp.V1.Commons;
using CatalogServiceAPI.Models;
using System;
using CatalogAPI.Service;
using NLog;
using NLog.Web;
using CatalogServiceAPI;

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
.GetCurrentClassLogger();
logger.Debug("init main");

try
{
    var builder = WebApplication.CreateBuilder(args);

    var configuration = builder.Configuration;

    var vaultService = new VaultService(configuration);


    string mySecret = await vaultService.GetSecretAsync("secrets", "SecretKey") ?? "5Jw9yT4fb9T5XrwKUz23QzA5D9BuY3p6";
    string myIssuer = await vaultService.GetSecretAsync("secrets", "IssuerKey") ?? "gAdDxQDQq7UYNxF3F8pLjVmGuU5u8g3y";
    string myConnectionString = await vaultService.GetSecretAsync("secrets", "MongoConnectionString") ?? "mongodb+srv://admin:1234@4semproj.npem60f.mongodb.net/";

    configuration["MongoConnectionString"] = myConnectionString;

    if (string.IsNullOrEmpty(myConnectionString))
    {
        logger.Error("ConnectionString not found in environment vaariables");
        throw new Exception("ConnectionString not found in environment variables");
    }
    else
    {
        logger.Info("ConnectionString: {0}", myConnectionString);
    }
    // Add services to the container.
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddControllers();
    builder.Services.AddTransient<VaultService>();
    builder.Services.AddSingleton<MongoDBContext>();
    builder.Services.AddSingleton<ICatalogInterface, CatalogMongoDBService>();

    // Configure JWT Authentication
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = myIssuer,
            ValidAudience = "http://localhost",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(mySecret)),
            ClockSkew = TimeSpan.Zero // hmmmmmm
        };

        // Tilføj event handler for OnAuthenticationFailed
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.Headers.Add("Token-Expired", "true");
                    logger.Error("Token expired: {0}", context.Exception.Message);
                }
                return Task.CompletedTask;
            }
        };
    });

    builder.Services.AddAuthorization(options =>
    {
        options.AddPolicy("UserRolePolicy", policy => policy.RequireRole("1"));
        options.AddPolicy("AdminRolePolicy", policy => policy.RequireRole("2"));
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowOrigin", builder =>
        {
            builder.AllowAnyHeader()
                   .AllowAnyMethod();
        });
    });

    builder.Services.AddControllers();
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.MapControllers();
    app.UseCors("AllowOrigin");
    app.UseHttpsRedirection();
    app.UseAuthentication();
    app.UseAuthorization();
    
    app.Run();
}
catch (Exception ex)
{
    logger.Error(ex, "Stopped program because of exception");
    throw;
}
finally
{
    NLog.LogManager.Shutdown();
}
