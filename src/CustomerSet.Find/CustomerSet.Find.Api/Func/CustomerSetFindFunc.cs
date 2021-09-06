using GGroupp.Infra;
using System.Collections.ObjectModel;

namespace GGroupp.Internal.Support;

internal sealed partial class CustomerSetFindFunc : IAsyncValueFunc<CustomerSetFindIn, Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>>
{
    private static readonly ReadOnlyCollection<string> selectedFields;

    static CustomerSetFindFunc()
        =>
        selectedFields = new(new[] { ApiJsonFieldName.Name, ApiJsonFieldName.AccountId });

    private readonly IDataverseEntitySetGetSupplier dataverseEntitySetGetSupplier;

    private CustomerSetFindFunc(IDataverseEntitySetGetSupplier dataverseEntitySetGetSupplier)
        =>
        this.dataverseEntitySetGetSupplier = dataverseEntitySetGetSupplier;

    public static CustomerSetFindFunc Create(IDataverseEntitySetGetSupplier dataverseEntitySetGetSupplier)
        =>
        new(
            dataverseEntitySetGetSupplier ?? throw new ArgumentNullException(nameof(dataverseEntitySetGetSupplier)));

    public partial ValueTask<Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>> InvokeAsync(
        CustomerSetFindIn input, CancellationToken cancellationToken = default);
}