using GGroupp.Infra;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

partial class ContactSetSearchFunc
{
    public partial ValueTask<Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>> InvokeAsync(
        ContactSetSearchIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            @in => new DataverseSearchIn($"*{@in.SearchText}*")
            {
                Entities = entities,
                Filter = $"parentcustomerid eq '{@in.CustomerId.ToString("D", CultureInfo.InvariantCulture)}'",
                Top = @in.Top 
            })
        .PipeValue(
            dataverseSearchSupplier.SearchAsync)
        .MapFailure(
            failure => failure.MapFailureCode(fail => ContactSetSearchFailureCode.Unknown))
        .MapSuccess(
            MapDataverseSearchOut);

    private static ContactSetSearchOut MapDataverseSearchOut(DataverseSearchOut dataverseSearchOut)
        =>
        new(
            dataverseSearchOut.Value
            .Select(
                item => new ContactItemSearchOut(
                    item.ObjectId, 
                    item.ExtensionData?.GetValueOrAbsent("fullname").OrDefault()?.ToString() ?? string.Empty))
            .ToArray());
}