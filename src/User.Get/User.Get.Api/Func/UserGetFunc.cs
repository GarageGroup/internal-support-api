using GGroupp.Infra;
using System.Collections.ObjectModel;

namespace GGroupp.Internal.Support;

internal sealed partial class UserGetFunc : IAsyncValueFunc<UserGetIn, Result<UserGetOut, Failure<UserGetFailureCode>>>
{
    private static readonly ReadOnlyCollection<string> selectedFields;

    static UserGetFunc()
        =>
        selectedFields = new(new[] { ApiNames.SystemUserId });

    private readonly IDataverseEntityGetSupplier entityGetSupplier;

    private UserGetFunc(IDataverseEntityGetSupplier entityGetSupplier)
        =>
        this.entityGetSupplier = entityGetSupplier;

    public static UserGetFunc Create(IDataverseEntityGetSupplier entityGetSupplier)
        =>
        new(
            entityGetSupplier ?? throw new ArgumentNullException(nameof(entityGetSupplier)));

    public partial ValueTask<Result<UserGetOut, Failure<UserGetFailureCode>>> InvokeAsync(
        UserGetIn input, CancellationToken cancellationToken = default);
}