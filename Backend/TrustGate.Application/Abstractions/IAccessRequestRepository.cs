using System;
using System.Collections.Generic;
using System.Text;
using TrustGate.Domain.Entities;

namespace TrustGate.Application.Abstractions
{
    public interface
        IAccessRequestRepository
    {
        Task<AccessRequest?> GetAsync(Guid id, CancellationToken ct);
        Task<bool> HasOpenRequestAsync(Guid userId, Guid entitlementId, CancellationToken ct);
        Task<List<AccessRequest>> GetPendingAsync(CancellationToken ct);
        void Add(AccessRequest request);
    }
}
