using System;
using System.Collections.Generic;
using System.Text;
using TrustGate.Domain.Entities;

namespace TrustGate.Application.Abstractions;

public interface IUserRepository
{
    Task<User?> GetAsync(Guid id, CancellationToken ct);
    Task<bool> UsernameExistsAsync(string username, CancellationToken ct);
    void Add(User user);
}
