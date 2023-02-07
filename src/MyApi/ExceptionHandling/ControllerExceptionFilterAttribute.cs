using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyApi.ExceptionHandling;

public class ControllerExceptionFilterAttribute : ExceptionFilterAttribute
{
     private const string ErrorKey = "Error";

    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is not UserFriendlyErrorPageException) return;

        HandleException(context);
        ProcessException(context);
    }

    void SetTraceId(string traceIdentifier, ProblemDetails problemDetails)
    {
        var traceId = Activity.Current?.Id ?? traceIdentifier;
        problemDetails.Extensions["traceId"] = traceId;
    }

    private void ProcessException(ExceptionContext context)
    {
        var problemDetails = new ValidationProblemDetails(context.ModelState)
        {
            Title = "One or more model validation errors occurred.",
            Status = StatusCodes.Status400BadRequest,
            Instance = context.HttpContext.Request.Path
        };
        
        SetTraceId(context.HttpContext.TraceIdentifier, problemDetails);

        var exceptionResult =  new BadRequestObjectResult(problemDetails)
        {
            ContentTypes = {
                "application/problem+json",
                "application/problem+xml" }
        };

        context.ExceptionHandled = true;
        context.Result = exceptionResult;
    }

    private void HandleException(ExceptionContext context)
    {
        if (context.Exception is UserFriendlyErrorPageException userFriendlyErrorPageException)
        {
            context.ModelState.AddModelError(userFriendlyErrorPageException.ErrorKey ?? ErrorKey, context.Exception.Message);
        }
    }
}