using GGroupp.Infra;
using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

using ICustomerSetFindFunc = IAsyncValueFunc<CustomerSetFindIn, Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>>;

internal sealed partial class CustomerSetFindFunc : ICustomerSetFindFunc
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