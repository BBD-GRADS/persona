using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PersonaBackend.Authentication;
using PersonaBackend.Database;
using PersonaBackend.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PersonaBackend.Database.IRepositories;
using PersonaBackend.Database.Repository;

namespace PersonaBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddSingleton<ApiKeyService>();

            // Add CORS policy
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("OpelCorsa",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               // builder.WithOrigins()
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();

            DotNetEnv.Env.Load(".env");

            var serverName = DotNetEnv.Env.GetString("SERVER_NAME")?.ToString();
            var databaseName = DotNetEnv.Env.GetString("DATABASE_NAME")?.ToString();
            var username = DotNetEnv.Env.GetString("USERNAME")?.ToString();
            var password = DotNetEnv.Env.GetString("PASSWORD")?.ToString();

            var connectionString = "Server=" + serverName + ";Port=5432;Database=" + databaseName + ";Username=" + username + ";Password=" + password;
            builder.Services.AddDbContext<PersonaDatabaseContext>(options =>
            {
                options.UseNpgsql(connectionString ??
                    throw new InvalidOperationException("Connection String not found or invalid"));
            });

            builder.Services.AddScoped<IUserRepository, UserRepository>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("OpelCorsa");

            //app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ApiKeyAuthMiddleware>();

            app.MapControllers();

            app.Run();
        }
    }
}