using GGroupp.Infra;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

partial class CustomerSetSearchFunc
{
    public partial ValueTask<Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>> InvokeAsync(
        CustomerSetSearchIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            @in => new DataverseSearchIn($"*{@in.SearchText}*")
            {
                Entities = entities,
                Top = @in.Top 
            })
        .PipeValue(
            dataverseSearchSupplier.SearchAsync)
        .MapFailure(
            failure => failure.MapFailureCode(_ => CustomerSetSearchFailureCode.Unknown))
        .MapSuccess(
            @out => new CustomerSetSearchOut(
                @out.Value.Select(MapCustomerItemSearchOut).ToArray()));

    private static CustomerItemSearchOut MapCustomerItemSearchOut(DataverseSearchItem item)
        =>
        new(
            id: item.ObjectId,
            title: item.ExtensionData.GetValueOrAbsent("name").OrDefault()?.ToString());
}