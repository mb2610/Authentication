namespace MyApi.Configuration.AuditLogging;

using Microsoft.AspNetCore.Http.Extensions;

public class ApiAuditAction : IAuditAction
{
    public ApiAuditAction(IHttpContextAccessor accessor)
    {
        Action = new
        {
            TraceIdentifier = accessor.HttpContext.TraceIdentifier,
            RequestUrl = accessor.HttpContext.Request.GetDisplayUrl(),
            HttpMethod = accessor.HttpContext.Request.Method
        };
    }
    
    public object Action { get; set; }
}