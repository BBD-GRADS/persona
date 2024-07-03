using Microsoft.OpenApi.Models;
using Microsoft.EntityFrameworkCore;
using PersonaBackend.Utils;
using PersonaBackend.Data;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using Swashbuckle.AspNetCore.Filters;
using Npgsql;

namespace PersonaBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddSingleton(AWSManagerService.Instance);
            builder.Services.AddSingleton(Chronos.Instance);
            builder.Services.AddHttpClient();

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
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
                c.EnableAnnotations();
                c.ExampleFilters();

                // Configure Swagger to use x-api-key header
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
                {
                    Description = "API Key needed to access the endpoints.",
                    Name = "x-api-key",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme"
                });
                c.OperationFilter<ConditionalOperationFilter>();
            });

            builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();

            #region DB setup

            DotNetEnv.Env.Load(".env");
            var host = DotNetEnv.Env.GetString("HOST")?.ToString();
            var databaseName = DotNetEnv.Env.GetString("DATABASE_NAME")?.ToString();
            var username = DotNetEnv.Env.GetString("USERNAME")?.ToString();
            var password = DotNetEnv.Env.GetString("PASSWORD")?.ToString();

            var connectionString = $"Host={host};Port=5432;Database={databaseName};Username={username};Password={password}";
            builder.Services.AddDbContext<Context>(options =>
            {
                options.UseNpgsql(connectionString ??
                    throw new InvalidOperationException("Connection String not found or invalid"));
            });

            #endregion DB setup

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //if (app.Environment.IsDevelopment())
            //{
            app.UseSwagger();
            app.UseSwaggerUI();
            //}

            app.UseHttpsRedirection();

            app.UseCors("OpelCorsa");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}