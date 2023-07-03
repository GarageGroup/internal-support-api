using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class SupportApi
{
    public ValueTask<Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>> SearchContactSetAsync(
        ContactSetSearchIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .HandleCancellation()
        .Pipe(
            static @in => new DataverseSearchIn($"*{@in.SearchText}*")
            {
                Entities = ContactSetSearchEntities,
                Filter = $"parentcustomerid eq '{@in.CustomerId:D}'",
                Top = @in.Top 
            })
        .PipeValue(
            dataverseApiClient.SearchAsync)
        .MapFailure(
            static failure => failure.MapFailureCode(ToContactSetSearchFailureCode))
        .MapSuccess(
            static @out => new ContactSetSearchOut
            {
                Contacts = @out.Value.Map(MapDataverseSearchItem)
            });

    private static ContactItemSearchOut MapDataverseSearchItem(DataverseSearchItem item)
        =>
        new(
            id: item.ObjectId,
            fullName: item.ExtensionData.AsEnumerable().GetValueOrAbsent("fullname").OrDefault()?.ToString());

    private static ContactSetSearchFailureCode ToContactSetSearchFailureCode(DataverseFailureCode failureCode)
        =>
        failureCode switch
        {
            DataverseFailureCode.UserNotEnabled => ContactSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.SearchableEntityNotFound => ContactSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.Throttling => ContactSetSearchFailureCode.TooManyRequests,
            _ => default
        };
}