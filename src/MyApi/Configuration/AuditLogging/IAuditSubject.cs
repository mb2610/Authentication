namespace MyApi.Configuration.AuditLogging;

public interface IAuditSubject
{
    public string SubjectName { get; set; }

    public string SubjectType { get; set; }

    public object SubjectAdditionalData { get; set; }

    public string SubjectIdentifier { get; set; }
}