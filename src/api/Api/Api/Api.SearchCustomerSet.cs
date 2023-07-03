using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class SupportApi
{
    public ValueTask<Result<CustomerSetSearchOut, Failure<CustomerSetSearchFailureCode>>> SearchCustomerSetAsync(
        CustomerSetSearchIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .HandleCancellation()
        .Pipe(
            static @in => new DataverseSearchIn($"*{@in.SearchText}*")
            {
                Entities = CustomerSetSearchEntities,
                Top = @in.Top 
            })
        .PipeValue(
            dataverseApiClient.SearchAsync)
        .MapFailure(
            static failure => failure.MapFailureCode(ToCustomerSetSearchFailureCode))
        .MapSuccess(
            static @out => new CustomerSetSearchOut
            {
                Customers = @out.Value.Map(MapCustomerItemSearchOut)
            });

    private static CustomerItemSearchOut MapCustomerItemSearchOut(DataverseSearchItem item)
        =>
        new(
            id: item.ObjectId,
            title: item.ExtensionData.AsEnumerable().GetValueOrAbsent("name").OrDefault()?.ToString());

    private static CustomerSetSearchFailureCode ToCustomerSetSearchFailureCode(DataverseFailureCode failureCode)
        =>
        failureCode switch
        {
            DataverseFailureCode.UserNotEnabled => CustomerSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.SearchableEntityNotFound => CustomerSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.Throttling => CustomerSetSearchFailureCode.TooManyRequests,
            _ => default
        };
}