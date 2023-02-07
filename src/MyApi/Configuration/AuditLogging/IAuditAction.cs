namespace MyApi.Configuration.AuditLogging;

public interface IAuditAction
{
    object Action { get; set; }
}