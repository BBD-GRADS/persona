using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PersonaBackend.Authentication;
using PersonaBackend.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text;

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