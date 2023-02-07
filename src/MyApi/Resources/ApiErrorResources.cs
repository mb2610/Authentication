using MyApi.Configuration.Constants;
using MyApi.ExceptionHandling;

namespace MyApi.Resources;

public class ApiErrorResources : IApiErrorResources
{
    public virtual ApiError CannotSetId()
    {
        return new ApiError
        {
            Code = nameof(CannotSetId),
            Description = ApiErrorResource.CannotSetId
        };
    }
}