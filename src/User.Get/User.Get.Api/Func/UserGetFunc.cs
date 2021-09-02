using GGroupp.Infra;
using System.Collections.ObjectModel;

namespace GGroupp.Internal.Support;

internal sealed partial class UserGetFunc : IAsyncValueFunc<UserGetIn, Result<UserGetOut, Failure<UserGetFailureCode>>>
{
    public static UserGetFunc Create(IDataverseEntityGetSupplier entityGetSupplier)
        =>
        new(
            entityGetSupplier ?? throw new ArgumentNullException(nameof(entityGetSupplier)));
    
    private static readonly ReadOnlyCollection<string> selectedFields;

    static UserGetFunc()
        =>
        selectedFields = new(new[] { ApiJsonFieldName.SystemUserId });

    private readonly IDataverseEntityGetSupplier entityGetSupplier;

    private UserGetFunc(IDataverseEntityGetSupplier entityGetSupplier)
        =>
        this.entityGetSupplier = entityGetSupplier;

    public partial ValueTask<Result<UserGetOut, Failure<UserGetFailureCode>>> InvokeAsync(
        UserGetIn input, CancellationToken cancellationToken = default);
}