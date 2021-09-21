using GGroupp.Infra;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

partial class CustomerSetFindFunc
{
    public partial ValueTask<Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>> InvokeAsync(
        CustomerSetFindIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Start(
            input ?? throw new ArgumentNullException(nameof(input)),
            cancellationToken)
        .Pipe(
            input => input.SearchText.IsNullOrEmpty() ? string.Empty : $"contains(name,'{input.SearchText}')")
        .Pipe(
            filter => new DataverseEntitySetGetIn(
                entitySetName: "accounts",
                selectFields: selectedFields,
                filter: filter))
        .PipeValue(
            dataverseEntitySetGetSupplier.GetEntitySetAsync<CustomerSetFindJsonOut>)
        .MapFailure(
            failure => failure.MapFailureCode(_ => CustomerSetFindFailureCode.Unknown))
        .MapSuccess(
            entitySetGetOut => new CustomerSetFindOut(
                entitySetGetOut.Value.Select(MapJsonOut).ToArray()));

    private static CustomerItemFindOut MapJsonOut(CustomerSetFindJsonOut jsonOut)
        =>
        new(id: jsonOut.Id, title: jsonOut.Title);
}