using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PersonaBackend.Authentication;
using PersonaBackend.Database;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;
using Microsoft.EntityFrameworkCore;
using PersonaBackend.Database.IRepositories;
using PersonaBackend.Database.Repository;
using PersonaBackend.Utils;
using Swashbuckle.AspNetCore.Filters;

namespace PersonaBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton(AWSSecretsManagerService.Instance);

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
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PersonaBackend API", Version = "v1" });

                // Configure Swagger to use x-api-key header
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key needed to access the endpoints. Example: 'Bearer {your token}'",
                    Name = "x-api-key",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme"
                });

                c.OperationFilter<ConditionalOperationFilter>();
            });

            #region DB setup

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

            #endregion DB setup

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("OpelCorsa");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}