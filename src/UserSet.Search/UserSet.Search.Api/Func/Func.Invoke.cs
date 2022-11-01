using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;

namespace GGroupp.Internal.Support;

partial class UserSetSearchFunc
{
    public ValueTask<Result<UserSetSearchOut, Failure<UserSetSearchFailureCode>>> InvokeAsync(
        UserSetSearchIn input, CancellationToken cancellationToken = default)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .HandleCancellation()
        .Pipe(
            static @in => new DataverseSearchIn("*" + @in.SearchText + "*")
            {
                Entities = entities,
                Top = @in.Top
            })
        .PipeValue(
            dataverseSearchSupplier.SearchAsync)
        .MapFailure(
            static failure => failure.MapFailureCode(MapFailureCode))
        .MapSuccess(
            static success => new UserSetSearchOut(
                success.Value.Select(MapUserItemSearchOut).ToFlatArray()));

    private static UserItemSearchOut MapUserItemSearchOut(DataverseSearchItem item)
        =>
        new(
            id: item.ObjectId,
            fullName: item.ExtensionData.GetValueOrAbsent("fullname").OrDefault()?.ToString());

    private static UserSetSearchFailureCode MapFailureCode(DataverseFailureCode failureCode)
        =>
        failureCode switch
        {
            DataverseFailureCode.UserNotEnabled => UserSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.SearchableEntityNotFound => UserSetSearchFailureCode.NotAllowed,
            DataverseFailureCode.Throttling => UserSetSearchFailureCode.TooManyRequests,
            _ => default
        };
}