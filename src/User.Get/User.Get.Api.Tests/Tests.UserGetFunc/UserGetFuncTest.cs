using GGroupp.Infra;
using Moq;
using PrimeFuncPack;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support.User.Get.Api.Tests;

using IUserGetFunc = IAsyncValueFunc<UserGetIn, Result<UserGetOut, Failure<UserGetFailureCode>>>;

public sealed partial class UserGetFuncTest
{
    static UserGetFuncTest()
    {
        SomeActiveDirectoryGuid = Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b7");
        SomeSystemUserId = Guid.Parse("bd8b8e33-554e-e611-80dc-c4346bad0190");
    }
    
    private static readonly Guid SomeActiveDirectoryGuid;
    
    private static readonly Guid SomeSystemUserId;

    private const int DataverseNotFoundStatusCode = -2147088239;

    private static IUserGetFunc CreateFunc(IDataverseEntityGetSupplier dataverseEntityGetSupplier)
        =>
        Dependency.Of(dataverseEntityGetSupplier)
        .UseUserGetApi()
        .Resolve(Mock.Of<IServiceProvider>());

    private static Mock<IDataverseEntityGetSupplier> CreateMockDataverseApiClient(
        Result<DataverseEntityGetOut<UserJsonGetOut>, Failure<int>> result,
        Action<DataverseEntityGetIn>? callback = default)
    {
        var mock = new Mock<IDataverseEntityGetSupplier>();

        var m = mock.Setup(
            s => s.GetEntityAsync<UserJsonGetOut>(It.IsAny<DataverseEntityGetIn>(), It.IsAny<CancellationToken>()))
            .Returns(result.Pipe(ValueTask.FromResult));

        if (callback is not null)
        {
            m.Callback<DataverseEntityGetIn, CancellationToken>(
                (@in, _) => callback.Invoke(@in));
        }

        return mock;
    }
}