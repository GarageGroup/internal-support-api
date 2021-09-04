using GGroupp.Infra;
using System.Globalization;

namespace GGroupp.Internal.Support;

partial class UserGetFunc
{
    public partial ValueTask<Result<UserGetOut, Failure<UserGetFailureCode>>> InvokeAsync(
        UserGetIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Start(
            input ?? throw new ArgumentNullException(nameof(input)),
            cancellationToken)
        .Pipe(
            @in => new DataverseEntityGetIn(
                entityPluralName: ApiNames.SystemUserEntity,
                entityKey: BuildAlternateKey(input.ActiveDirectoryUserId),
                selectFields: selectedFields))
        .PipeValue(
                entityGetSupplier.GetEntityAsync<UserGetJsonOut>)
        .MapFailure(
            MapDataverseFailureCode)
        .MapSuccess(
            entityGetOut => new UserGetOut(entityGetOut?.Value?.SystemUserId ?? default));

    private static DataverseAlternateKey BuildAlternateKey(Guid activeDirectoryId)
        => 
        new(
            new KeyValuePair<string, string>[]
            {
                new(ApiNames.ActiveDirectoryObjectId, activeDirectoryId.ToString("D", CultureInfo.InvariantCulture))
            });

    private static Failure<UserGetFailureCode> MapDataverseFailureCode(Failure<int> dataverseFailureCode)
        =>
        dataverseFailureCode switch
        {
            { FailureCode: ApiNames.NotFoundFailureCode } => dataverseFailureCode.MapFailureCode(_ => UserGetFailureCode.NotFound),
            _ => dataverseFailureCode.MapFailureCode(_ => UserGetFailureCode.Unknown)
        };
}