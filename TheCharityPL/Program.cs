using Microsoft.OpenApi.Models;
using System.Reflection;
using TheCharityBLL.Helpers;
using TheCharityPL.Middlewares;

namespace TheCharityPL
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.TheCharityEnhancedConnectionString(builder.Configuration);
            builder.Services.TheCharityDependencyInjection();
            builder.Services.TheCharityIdentity(builder.Configuration);
            builder.Services.FoxArtEmailConfiguration(builder.Configuration);
            builder.Services.ThirdPartyAuthentication(builder.Configuration);
            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                // This prevents unknown properties from crashing deserialization
            }); ;
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "The Charity API",
                    Version = "v1"
                });

                // Load XML from TheCharityPL (controllers)
                var plXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var plXmlPath = Path.Combine(AppContext.BaseDirectory, plXmlFile);
                if (File.Exists(plXmlPath))
                {
                    c.IncludeXmlComments(plXmlPath);
                }

                // Load XML from TheCharityBLL (DTOs)
                var bllXmlFile = "TheCharityBLL.xml";
                var bllXmlPath = Path.Combine(AppContext.BaseDirectory, bllXmlFile);
                if (File.Exists(bllXmlPath))
                {
                    c.IncludeXmlComments(bllXmlPath);
                }
            });

            var app = builder.Build();

            app.MapHealthChecks("/health");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //global exception handling middleware
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}