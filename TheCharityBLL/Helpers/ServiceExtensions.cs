using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TheCharityBLL.Services.Abstraction;
using TheCharityDAL.Database;
using TheCharityDAL.Entities;
using TheCharityDAL.Repositories.Abstraction;
using TheCharityDAL.Repositories.Implementation;
using TheCharityBLL.Services.Repository;
using TheCharityBLL.Mapper;
using TheCharityBLL.Settings;
using Microsoft.AspNetCore.Authentication.Cookies;
namespace TheCharityBLL.Helpers
{
    public static class ServiceExtensions
    {
        public static void TheCharityIdentity(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddDataProtection();
            services.AddIdentityCore<User>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 6;
            })
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<TheCharityDbContext>()
            .AddDefaultTokenProviders();



        }
        public static void FoxArtEmailConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));
        }
        public static void TheCharityEnhancedConnectionString(this IServiceCollection services, IConfiguration configuration, string stringName = "defaultConnection")
        {
            var connectionString = configuration.GetConnectionString(stringName);
            services.AddDbContext<TheCharityDbContext>(options =>
                options.UseSqlServer(
                    connectionString,
                    b => b.MigrationsAssembly("TheCharityDAL")
                    ));
            services.AddHealthChecks()
        .AddSqlServer(
            connectionString: connectionString,
            name: "TheCharity-DB",
            failureStatus: HealthStatus.Unhealthy,
            tags: new[] { "db", "sql", "charity" }
        );
        }
        public static void TheCharityDependencyInjection(this IServiceCollection services)
        {
            // Repository Injection
            services.AddScoped<ICampaignRepository, CampaignRepository>();
            services.AddScoped<IDonatedItemsRepository, DonatedItemsRepository>();
            services.AddScoped<IDonationRepository, DonationRepository>();
            services.AddScoped<IOrganizationRepository, OrganizationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Services Injection
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPaymobService, PaymobService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IOrganizationQueryService, OrganizationQueryService>();
            services.AddScoped<IOrganizationContactService, OrganizationContactService>();
            services.AddScoped<IDonatedItemService, DonatedItemService>();
            services.AddScoped<IDonatedItemQueryService, DonatedItemQueryService>();
            services.AddScoped<IDonatedItemAnalyticsService, DonatedItemAnalyticsService>();
            services.AddScoped<IDonatedItemAttachmentService, DonatedItemAttachmentService>();
            services.AddScoped<IDonatedItemImageService, DonatedItemImageService>();
            // mapper Injection
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

        }
        public static void ThirdPartyAuthentication(this IServiceCollection services, IConfiguration Configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = Configuration["Jwt:Issuer"],
                ValidAudience = Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured")))
            };
        })
        .AddCookie("ExternalCookie") // dedicated scheme for OAuth handshake only
        .AddGoogle(options =>
        {
            options.SignInScheme = "ExternalCookie"; // scoped here only
            options.ClientId = Configuration["Authentication:Google:ClientID"];
            options.ClientSecret = Configuration["Authentication:Google:SecretKey"];
        })
        .AddFacebook(options =>
        {
            options.SignInScheme = "ExternalCookie"; // scoped here only
            options.AppId = Configuration["Authentication:Facebook:ClientID"];
            options.AppSecret = Configuration["Authentication:Facebook:SecretKey"];
        });
        }
    }
}       
