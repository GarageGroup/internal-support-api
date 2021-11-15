using GGroupp.Infra;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support;

partial class UserGetFunc
{
    public partial ValueTask<Result<UserGetOut, Failure<UserGetFailureCode>>> InvokeAsync(
        UserGetIn input, CancellationToken cancellationToken)
        =>
        AsyncPipeline.Pipe(
            input, cancellationToken)
        .Pipe(
            @in => new DataverseEntityGetIn(
                entityPluralName: ApiNames.SystemUserEntity,
                entityKey: BuildAlternateKey(input.ActiveDirectoryUserId),
                selectFields: selectedFields))
        .PipeValue(
            entityGetSupplier.GetEntityAsync<UserJsonGetOut>)
        .MapFailure(
            failure => failure.MapFailureCode(MapDataverseFailureCode))
        .MapSuccess(
            entityGetOut => new UserGetOut(entityGetOut.Value.SystemUserId));

    private static DataverseAlternateKey BuildAlternateKey(Guid activeDirectoryId)
        => 
        new(
            new KeyValuePair<string, string>[]
            {
                new(ApiNames.ActiveDirectoryObjectId, activeDirectoryId.ToString("D", CultureInfo.InvariantCulture))
            });

    private static UserGetFailureCode MapDataverseFailureCode(int dataverseFailureCode)
        =>
        dataverseFailureCode switch
        {
            ApiNames.NotFoundFailureCode => UserGetFailureCode.NotFound,
            _ => UserGetFailureCode.Unknown
        };
}