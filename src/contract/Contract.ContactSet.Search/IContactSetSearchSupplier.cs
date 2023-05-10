using System;
using System.Threading;
using System.Threading.Tasks;

namespace GarageGroup.Internal.Support;

public interface IContactSetSearchSupplier
{
    ValueTask<Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>> SearchContactSetAsync(
        ContactSetSearchIn input, CancellationToken cancellationToken);
}