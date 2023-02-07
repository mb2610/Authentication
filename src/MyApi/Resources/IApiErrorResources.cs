using MyApi.ExceptionHandling;

namespace MyApi.Resources;

public interface IApiErrorResources
{
    ApiError CannotSetId();
}