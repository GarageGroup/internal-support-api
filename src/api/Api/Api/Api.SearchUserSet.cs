using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;

namespace GarageGroup.Internal.Support;

partial class SupportApi
{
    public ValueTask<Result<UserSetSearchOut, Failure<UserSetSearchFailureCode>>> SearchUserSetAsync(
        UserSetSearchIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input ?? throw new ArgumentNullException(nameof(input)), cancellationToken)
        .HandleCancellation()
        .Pipe(
            static @in => new DataverseSearchIn("*" + @in.SearchText + "*")
            {
                Entities = UseSetSearchEntities,
                Top = @in.Top
            })
        .PipeValue(
            dataverseApiClient.SearchAsync)
        .MapFailure(
            static failure => failure.MapFailureCode(ToUserSetSearchFailureCode))
        .MapSuccess(
            static success => new UserSetSearchOut
            {
                Users = success.Value.Map(MapUserItemSearchOut)
            });

    private static UserItemSearchOut MapUserItemSearchOut(DataverseSearchItem item)
        =>
        new(
            id: item.ObjectId,
            fullName: item.ExtensionData.AsEnumerable().GetValueOrAbsent("fullname").OrDefault()?.ToString());

    private static UserSetSearchFailureCode ToUserSetSearchFailureCode(DataverseFailureCode failureCode)
        =>
        failureCode switch
        {
            DataverseFailureCode.UserNotEnabled => UserSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.SearchableEntityNotFound => UserSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.Throttling => UserSetSearchFailureCode.TooManyRequests,
            _ => default
        };
}