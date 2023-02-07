using System.Reflection;
using System.Security.Claims;
using IdentityModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MyApi.Configuration;
using MyApi.Configuration.DbContexts;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace MyApi.Helpers;

public static class ConfigureServicesHelpers
{
    public static void RegisterDbContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DbConnection");
        var migrationsAssembly = typeof (Startup).GetTypeInfo().Assembly.GetName().Name;
        services.AddDbContext<BlogDbContext>((Action<DbContextOptionsBuilder>) (options => options.UseNpgsql(connectionString, (Action<NpgsqlDbContextOptionsBuilder>) (sql => sql.MigrationsAssembly(migrationsAssembly)))));
    }
    
    public static void RegisterAuthentication(this IServiceCollection services)
    {
        var apiConfiguration = services.BuildServiceProvider().GetService<ApiConfiguration>();

        // services
        //     .AddIdentity<TUser, TRole>(options => configuration.GetSection(nameof(IdentityOptions)).Bind(options))
        //     .AddEntityFrameworkStores<TIdentityDbContext>()
        //     .AddDefaultTokenProviders();

        services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            // .AddCookie()
            .AddJwtBearer(opt =>
            {
                opt.Authority = apiConfiguration.IdentityServerBaseUrl;
                opt.RequireHttpsMetadata = apiConfiguration.RequireHttpsMetadata;
                opt.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = apiConfiguration.IdentityServerBaseUrl,
                    ValidateAudience = true,
                    ValidAudience = "account",
                    ValidateIssuerSigningKey = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
               opt.Events = AuthEventsHandler.Instance;
            });
    }
    
    public class AuthEventsHandler : JwtBearerEvents
    {
        private const string BearerPrefix = "Bearer ";
    
        private AuthEventsHandler() => OnMessageReceived = MessageReceivedHandler;
    
        /// <summary>
        /// Gets single available instance of <see cref="AuthEventsHandler"/>
        /// </summary>
        public static AuthEventsHandler Instance { get; } = new AuthEventsHandler();
    
        private Task MessageReceivedHandler(MessageReceivedContext context)
        {
            if (context.Request.Headers.TryGetValue("Authorization", out StringValues headerValue))
            {
                string token = headerValue;
                if (!string.IsNullOrEmpty(token) && token.StartsWith(BearerPrefix))
                {
                    token = token.Substring(BearerPrefix.Length);
                }
    
                context.Token = token;
            }
    
            return Task.CompletedTask;
        }
    }
    
    public static void RegisterAuthorization(this IServiceCollection services)
    {
        var apiConfiguration = services.BuildServiceProvider().GetService<ApiConfiguration>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy("admin",
                policy =>
                {
                    policy.RequireAssertion(context => context.User.HasClaim(c =>
                            ((c.Type == JwtClaimTypes.Role && c.Value == "admin") ||
                             (c.Type == $"client_{JwtClaimTypes.Role}" && c.Value == "Admin"))
                        ) && context.User.HasClaim(c =>
                            c.Type == JwtClaimTypes.Scope && c.Value == apiConfiguration.OidcApiName)
                    );
                });
        });
    }
    
    public static void RegisterSwagger(this IServiceCollection services)
    {
        var apiConfiguration = services.BuildServiceProvider().GetService<ApiConfiguration>();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

        services.AddEndpointsApiExplorer();
        // services.AddTransient<IConfigureOptions<SwaggerGenOptionsExtensions>>()
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc(apiConfiguration.ApiVersion, new OpenApiInfo { Title = apiConfiguration.ApiName, Version = apiConfiguration.ApiVersion });
            
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                      Enter 'Bearer' [space] and then your token in the text input below.
                      \r\n\r\nExample: 'Bearer 12345abcdef'",
                Type = SecuritySchemeType.Http,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Scheme = JwtBearerDefaults.AuthenticationScheme,
                BearerFormat = "JWT",
                Reference = new OpenApiReference
                {
                    Id = JwtBearerDefaults.AuthenticationScheme,
                    Type = ReferenceType.SecurityScheme
                }
            });
            // options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
            // {
            //     Type = SecuritySchemeType.OAuth2,
            //     Flows = new OpenApiOAuthFlows
            //     {
            //         AuthorizationCode = new OpenApiOAuthFlow
            //         {
            //             AuthorizationUrl = new Uri($"{apiConfiguration.IdentityServerBaseUrl}/connect/authorize"),
            //             TokenUrl = new Uri($"{apiConfiguration.IdentityServerBaseUrl}/connect/token"),
            //             Scopes = new Dictionary<string, string> {
            //                 { apiConfiguration.OidcApiName, apiConfiguration.ApiName }
            //             }
            //         }
            //     }
            // });
            // options.OperationFilter<AuthorizeCheckOperationFilter>();
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header,

                    },
                    new List<string>()
                }
            });
        });
    }
    
    public static void RegisterCors(this IServiceCollection services)
    {
        var apiConfiguration = services.BuildServiceProvider().GetService<ApiConfiguration>();
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(
                builder =>
                {
                    if (apiConfiguration.CorsAllowAnyOrigin)
                    {
                        builder.AllowAnyOrigin();
                    }
                    else
                    {
                        builder.WithOrigins(apiConfiguration.CorsAllowOrigins);
                    }

                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                });
        });
    }

    public static void RegisterHealthChecks(this IServiceCollection services, IConfiguration configuration)
    {
        var apiConfiguration = services.BuildServiceProvider().GetService<ApiConfiguration>();
        var connectionString = configuration.GetConnectionString("DbConnection");
        var identityServerUri = apiConfiguration.IdentityServerBaseUrl;
        var healthChecksBuilder = services.AddHealthChecks()
            .AddDbContextCheck<BlogDbContext>("BlogDbContext")
            .AddIdentityServer(new Uri(identityServerUri), "Identity Server");

        var serviceProvider = services.BuildServiceProvider();
        var scopeFactory = serviceProvider.GetRequiredService<IServiceScopeFactory>();
        using (var scope = scopeFactory.CreateScope())
        {
            var service = scope.ServiceProvider.GetService<BlogDbContext>();
            if (service == null) return;
            var entityType = service.Model.GetEntityTypes().FirstOrDefault();
            var tableName = entityType?.GetTableName() ?? string.Empty;
            healthChecksBuilder.AddNpgSql(connectionString, name: "DataProtectionDb", healthQuery: $"SELECT * FROM \"{tableName}\"  LIMIT 1");
        }
    }
}