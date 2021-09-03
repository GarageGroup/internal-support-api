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
                entityPluralName: ApiConstants.SystemUserEntity,
                entityKey: BuildAlternateKey(input.ActiveDirectoryUserId),
                selectFields: selectedFields))
        .PipeValue(
                entityGetSupplier.GetEntityAsync<UserGetJsonOut>)
        .MapFailure(
            failure => failure.MapFailureCode(_ => UserGetFailureCode.Unknown))
        .MapSuccess(
            entityGetOut => new UserGetOut(entityGetOut?.Value?.SystemUserId ?? default));

    private static DataverseAlternateKey BuildAlternateKey(Guid activeDirectoryId)
        => 
        new(
            new KeyValuePair<string, string>[]
            {
                new(ApiConstants.ActiveDirectoryObjectId, activeDirectoryId.ToString("D", CultureInfo.InvariantCulture))
            });
}