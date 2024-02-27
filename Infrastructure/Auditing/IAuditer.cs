namespace Infrastructure.Auditing;

public interface IAuditer
{
    Task AuditClaim(string id, string httpRequestType, CancellationToken cancellationToken);
    Task AuditCover(string id, string httpRequestType, CancellationToken cancellationToken);
}