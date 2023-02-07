namespace MyApi.ExceptionHandling;

public class UserFriendlyErrorPageException : Exception
{
    public string ErrorKey { get; set; }

    public UserFriendlyErrorPageException(string message)
        : base(message)
    {
    }

    public UserFriendlyErrorPageException(string message, string errorKey)
        : base(message)
    {
        this.ErrorKey = errorKey;
    }

    public UserFriendlyErrorPageException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}