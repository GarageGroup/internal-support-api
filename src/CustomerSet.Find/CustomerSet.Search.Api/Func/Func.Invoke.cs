using GGroupp.Infra;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

partial class CustomerSetSearchFunc
{
    public partial ValueTask<Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>> InvokeAsync(
        CustomerSetFindIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)),
            cancellationToken)
        .Pipe(
            @in => new DataverseSearchIn($"*{@in.SearchText}*")
            {
                Entities = entities,
                Top = @in.Top 
            })
        .PipeValue(
            dataverseSearchSupplier.SearchAsync)
        .MapFailure(
            failure => failure.MapFailureCode(fail => CustomerSetFindFailureCode.Unknown))
        .MapSuccess(
            MapDataverseSearchOut);

    private static CustomerSetFindOut MapDataverseSearchOut(DataverseSearchOut dataverseSearchOut)
        =>
        new(
            dataverseSearchOut.Value
            .Select(
                item => new CustomerItemFindOut(
                    item.ObjectId, 
                    item.ExtensionData?.GetValueOrAbsent("name").OrDefault()?.ToString() ?? string.Empty))
            .ToArray());
}