using System;
using System.Collections.Generic;
using System.Text;

namespace TrustGate.Domain.Entities
{
    public class UserEntitlement
    {
        public Guid UserId { get; private set; }
        public Guid EntitlementId { get; private set; }
        public Guid GrantedByRequestId { get; private set; }   // provenance: every grant traces to a request
        public DateTimeOffset GrantedAt { get; private set; } = DateTimeOffset.UtcNow;

        private UserEntitlement() { }
        public UserEntitlement(Guid userId, Guid entitlementId, Guid grantedByRequestId)
            => (UserId, EntitlementId, GrantedByRequestId) = (userId, entitlementId, grantedByRequestId);
    }
}
