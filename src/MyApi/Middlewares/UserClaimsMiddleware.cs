using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuGet.Versioning;

namespace MyApi.Middlewares;

public class UserClaimsMiddleware
{
    private readonly RequestDelegate _next;

    public UserClaimsMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        if (httpContext.User != null && httpContext.User.Identity.IsAuthenticated)
        {
            var claims = new List<Claim>();
            // var ressource = httpContext.User.Claims.First("resource_access");

            var sid = httpContext.User.FindFirstValue("sid");
            claims.Add(new Claim(ClaimTypes.NameIdentifier, sid, ClaimValueTypes.String));
            var resourceAccess = httpContext.User.FindFirstValue("resource_access");
            dynamic parse = JObject.Parse(resourceAccess);
            foreach (var role in parse.account.roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role, ClaimValueTypes.String));
            }
            var appIdentity = new ClaimsIdentity(claims);
            httpContext.User.AddIdentity(appIdentity);

            await _next(httpContext);
        }
    }
}

public class ResourceAccess
{
    public ResourceRole Account { get; set; }
    public ResourceRole ReactAuth { get; set; }
}

public class ResourceRole
{
    public string[] Roles { get; set; }
}

public static class UserClaimsMiddlewareExtensions
{
    public static IApplicationBuilder UseUserClaims(
        this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserClaimsMiddleware>();
    }
}