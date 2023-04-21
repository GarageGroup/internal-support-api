using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

public interface ICustomerSetSearchSupplier
{
    ValueTask<Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>> SearchCustomerSetAsync(
        CustomerSetSearchIn input, CancellationToken cancellationToken);
}