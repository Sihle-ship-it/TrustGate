using System;
using System.Collections.Generic;
using System.Text;
using TrustGate.Domain.Enums;

namespace TrustGate.Domain.Entities;

public class User
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Username { get; private set; }
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }   // hash only; hashing itself is Part 5
    public UserStatus Status { get; private set; } = UserStatus.Active;
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    private readonly List<UserEntitlement> _entitlements = new();
    public IReadOnlyCollection<UserEntitlement> Entitlements => _entitlements;

    private User() { }

    public User(string username, string email, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new DomainException("Username is required");
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@')) throw new DomainException("Valid email is required");
        Username = username.Trim();
        Email = email.Trim().ToLowerInvariant();
        PasswordHash = passwordHash;
    }

    public void Deactivate()
    {
        if (Status == UserStatus.Deactivated) throw new DomainException("User is already deactivated");
        Status = UserStatus.Deactivated;
        _entitlements.Clear();
    }

    public void GrantEntitlement(Guid entitlementId, Guid grantedByRequestId)
    {
        if (Status != UserStatus.Active) throw new DomainException("Cannot grant access to a non-active user");
        if (_entilements.Any(e => e.EntilementId == entitlementId))
            throw new DomainException("User already holds this entitlement.");
        _entitlements.Add(new UserEntitlement(Id, entitlementId, grantedByRequestId));
    }
}
