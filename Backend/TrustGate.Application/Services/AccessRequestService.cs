using System;
using System.Collections.Generic;
using System.Text;
using TrustGate.Application.Abstractions;
using TrustGate.Domain;
using TrustGate.Domain.Entities;
using TrustGate.Domain.Enums;

namespace TrustGate.Application.Services;

public class AccessRequestService
{
    private readonly IAccessRequestRepository _requests;
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;

    public AccessRequestService(IAccessRequestRepository requests, IUserRepository users, IUnitOfWork uow) =>
        (_requests, _users, _uow) = (requests, users, uow);

    public async Task<AccessRequest> SubmitAsync(Guid requesterId, Guid entitlementId, string justification, CancellationToken ct)
    {
        var user = await _users.GetAsync(requesterId,ct)
            ?? throw new DomainException("Requester not found.");
        if (user.Status != UserStatus.Active)
            throw new DomainException("Only active users can request access.");
        if (user.Entitlements.Any(e => e.EntitlementId == entitlementId))
            throw new DomainException("User already holds this entitlement.");
        if (await _requests.HasOpenRequestAsync(requesterId, entitlementId, ct))
            throw new DomainException("An open request for this entitlement already exists.");

        var request = new AccessRequest(requesterId, entitlementId, justification);
        _requests.Add(request);
        await _uow.SaveChangesAsync(ct);
        return request;

    }

    public async Task<AccessRequest> ApproveAsync(Guid requestId, Guid approverId, string comment,  CancellationToken ct)
    {
        var request = _requests.GetAsync(requestId, ct)
            ?? throw new DomainException("Request not found");
        var requester = _users.GetAsync(request.RequestId, ct)
            ?? throw new DomainException("Requester not found.");

        request.Approve(approverId, comment);
        requester.GrantEntitlement(request.EntitlementId, request.Id);

        await _uow.SaveChangesAsync(ct);

        return request;
    }

    public async Task<AccessRequest> RejectAsync(Guid requestId, Guid approverId, string comment, CancellationToken ct)
    {
        var request = _requests.GetAsync(requestId, ct)
            ?? throw new DomainException("Request not found");
        request.Reject(approverId, comment);

        await _uow.SaveChangesAsync(ct);

        return request;

    }


}
