using GGroupp.Infra;

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
            @in => new DataverseEntitySetGetIn(
                entitySetName: "accounts",
                selectFields: selectedFields,
                filter: $"contains(name,'{@in.SearchText}')"))
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