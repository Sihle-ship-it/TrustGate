using System;
using System.Collections.Generic;
using System.Text;
using TrustGate.Domain.Enums;

namespace TrustGate.Domain.Entities;

public class AccessRequest
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid RequesterId { get; private set; }
    public Guid EntitlementId { get; private set; }
    public string Justification { get; private set; }
    public AccessRequestStatus Status { get; private set; } = AccessRequestStatus.Submitted;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    public Guid? DecidedById { get; private set; }
    public DateTimeOffset? DecidedAt { get; private set; }
    public string? DecisionComment { get; private set; }

    public AccessRequest()
    {

    }

    public AccessRequest(Guid requesterId, Guid entitlementId, string justification)
    {
        if (string.IsNullOrWhiteSpace(justification) || justification.Trim().Length < 10)
            throw new DomainException("A justification of at least 10 characters is required.");
        RequesterId = requesterId;
        EntitlementId = entitlementId;
        Justification = justification.Trim();
    }

    public void Approve(Guid approverId, string comment)
    {
        EnsureDecidable();
        if (approverId == RequesterId)
            throw new DomainException("Requesters cannot approve thier own request");
        Status = AccessRequestStatus.Approved;
        RecordDecision(approverId, comment);
    }

    public void Reject(Guid approverId, string comment)
    {
        EnsureDecidable();
        if (string.IsNullOrWhiteSpace(comment))
            throw new DomainException("A rejection must include a reason.");
        Status = AccessRequestStatus.Rejected;
        RecordDecision(approverId, comment);
    }

    private void EnsureDecidable()
    {
        if (Status is not (AccessRequestStatus.Submitted or AccessRequestStatus.UnderReview
                or AccessRequestStatus.Escalated))
            throw new DomainException($"Request in status {Status} cannot be decided");
    }

    private void RecordDecision(Guid deciderId, string comment)
    {
        DecidedById = deciderId;
        DecidedAt = DateTime.UtcNow;
        DecisionComment = comment;

    }
}
