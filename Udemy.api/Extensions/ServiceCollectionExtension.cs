using System.Reflection;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Udemy.api.Data;
using Udemy.api.Models.Domain;
using Udemy.api.Repositories;
using Udemy.api.Services;
using Udemy.api.Settings;

namespace Udemy.api.Extensions;

public static class ServiceCollectionExtension
{
    public static void AddEssentials(this IServiceCollection services, IConfiguration configuration)
    {
        //var conStr = configuration.GetConnectionString("DefaultConnection");
        //services.AddDbContext<DataContext>(options => options.UseSqlServer(conStr));

        var conStr = configuration.GetConnectionString("SqliteConnection");
        services.AddDbContext<DataContext>(options => options.UseSqlite(conStr));
        services.AddIdentity<ApplicationUser, IdentityRole>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequireNonAlphanumeric = false;
        }).AddEntityFrameworkStores<DataContext>().AddDefaultTokenProviders();
        services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
        services.AddTransient<IMailService, SMTPMailService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<ICategoryRepository, CategoryRepository>();
        services.AddTransient<IProductRepository, ProductRepository>();
        services.AddTransient<IImageRepository, ImageRepository>();

        services.AddAutoMapper(Assembly.GetExecutingAssembly());
        services.AddHttpContextAccessor();
        services.RegisterSwagger();
        services.AddVersioning();
        services.AddJwtConfig(configuration);
    }
    private static void RegisterSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.IncludeXmlComments(string.Format(@"{0}\Udemy.Api.xml", AppDomain.CurrentDomain.BaseDirectory));
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Udemy Api",
                License = new OpenApiLicense()
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                Description = "Input your Bearer token in this format - bearer {your token} to access this API",
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme{
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer",
                        },
                        Scheme = "Bearer",
                        Name = "Bearer",
                        In = ParameterLocation.Header,
                    },
                    new List<string>()
                },
            });
        });
    }

    private static void AddVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(config =>
        {
            config.DefaultApiVersion = new ApiVersion(1, 0);
            config.AssumeDefaultVersionWhenUnspecified = true;
            config.ReportApiVersions = true;
        });
    }

    private static void AddJwtConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));
        services
            .AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
             {
                 o.RequireHttpsMetadata = false;
                 o.SaveToken = false;
                 o.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuerSigningKey = true,
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ClockSkew = TimeSpan.Zero,
                     ValidIssuer = configuration["JWTSettings:Issuer"],
                     ValidAudience = configuration["JWTSettings:Audience"],
                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                 };
                 o.Events = new JwtBearerEvents()
                 {
                     OnAuthenticationFailed = c =>
                     {
                         c.NoResult();
                         c.Response.StatusCode = 500;
                         c.Response.ContentType = "text/plain";
                         return c.Response.WriteAsync(c.Exception.ToString());
                     },
                     OnChallenge = con =>
                     {
                         con.HandleResponse();
                         con.Response.StatusCode = 401;
                         con.Response.ContentType = "application/json";
                         return con.Response.WriteAsync(JsonSerializer.Serialize("You Are Not Authorized"));
                     },
                     OnForbidden = con =>
                     {
                         con.Response.StatusCode = 403;
                         con.Response.ContentType = "application/json";
                         return con.Response.WriteAsync(JsonSerializer.Serialize("You Are not Autorized to get this resource"));
                     },
                 };
             });
    }
}
