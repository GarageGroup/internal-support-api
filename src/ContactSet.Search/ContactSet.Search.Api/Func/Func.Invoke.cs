using GGroupp.Infra;
using System;
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
            failure => failure.MapFailureCode(MapFailureCode))
        .MapSuccess(
            @out => new ContactSetSearchOut(
                @out.Value.Select(MapDataverseSearchItem).ToArray()));

    private static ContactItemSearchOut MapDataverseSearchItem(DataverseSearchItem item)
        =>
        new(
            id: item.ObjectId,
            fullName: item.ExtensionData.GetValueOrAbsent("fullname").OrDefault()?.ToString());

    private static ContactSetSearchFailureCode MapFailureCode(DataverseFailureCode failureCode)
        =>
        failureCode switch
        {
            DataverseFailureCode.UserNotEnabled => ContactSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.SearchableEntityNotFound => ContactSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.Throttling => ContactSetSearchFailureCode.TooManyRequests,
            _ => default
        };
}