using System;
using System.Collections.Generic;
using System.Text;

namespace TrustGate.Application.Abstractions;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct);
}
