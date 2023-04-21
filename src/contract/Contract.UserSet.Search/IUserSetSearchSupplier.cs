using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

public interface IUserSetSearchSupplier
{
    ValueTask<Result<UserSetSearchOut, Failure<UserSetSearchFailureCode>>> SearchUserSetAsync(
        UserSetSearchIn input, CancellationToken cancellationToken);
}