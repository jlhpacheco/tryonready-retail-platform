namespace TryOnReady.Domain.Auditing;

public sealed record AuditEvent(
    Guid Id,
    string EventType,
    string SubjectType,
    Guid SubjectId,
    DateTimeOffset OccurredAtUtc);
