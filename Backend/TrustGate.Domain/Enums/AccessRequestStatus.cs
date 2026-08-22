using System;
using System.Collections.Generic;
using System.Text;

namespace TrustGate.Domain.Enums;

public enum AccessRequestStatus
{
    Draft, Submitted, UnderReview, Approved, Rejected, Escalated, Exception, Closed
}
