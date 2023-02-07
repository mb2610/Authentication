using System.IdentityModel.Tokens.Jwt;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MyApi.Configuration;
using MyApi.ExceptionHandling;
using MyApi.Helpers;
using MyApi.Middlewares;
using MyApi.Resources;

namespace MyApi;

public class Startup
{
    public IConfiguration Configuration { get; }
    public IWebHostEnvironment HostingEnvironment { get; }
    
    public Startup(IWebHostEnvironment env, IConfiguration configuration)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
        HostingEnvironment = env;
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        var apiConfiguration = Configuration.GetSection(nameof(ApiConfiguration)).Get<ApiConfiguration>();
        services.AddSingleton(apiConfiguration);

        services.AddControllers();
        
        // Add DbContexts
        services.RegisterDbContexts(Configuration);

        services.AddScoped<ControllerExceptionFilterAttribute>();
        services.AddScoped<IApiErrorResources, ApiErrorResources>();

        // Add authentication services
        services.RegisterAuthentication();

        // Add authorization services
        services.RegisterAuthorization();

        // Add Cors
        services.RegisterCors();

        // Add Swagger
        services.RegisterSwagger();

        // Add Logging
        // services.RegisterLogging();

        // Add Health Check
        services.RegisterHealthChecks(Configuration);
    }


    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApiConfiguration apiConfiguration)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseHttpsRedirection();
        app.RegisterSwagger(apiConfiguration);
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseUserClaims();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();

            endpoints.MapHealthChecks("/health", new HealthCheckOptions
            {
                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
            });
        });
    }
}