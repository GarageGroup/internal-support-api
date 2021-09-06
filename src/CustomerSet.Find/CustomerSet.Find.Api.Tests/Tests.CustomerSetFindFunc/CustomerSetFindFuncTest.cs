using GGroupp.Infra;
using Moq;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.CustomerSet.Find.Api.Tests;

using ICustomerSetFindFunc = IAsyncValueFunc<CustomerSetFindIn, Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>>;

public sealed partial class CustomerSetFindFuncTest
{
    private static ICustomerSetFindFunc CreateFunc(IDataverseEntitySetGetSupplier dataverseEntitySetGetSupplier)
        =>
        Dependency.Create(_ => dataverseEntitySetGetSupplier)
        .UseCustomerSetFindApi()
        .Resolve(Mock.Of<IServiceProvider>());

    private static Mock<IDataverseEntitySetGetSupplier> CreateMockDataverseApiClient(
        Result<DataverseEntitySetGetOut<CustomerSetFindJsonOut>, Failure<int>> result,
        Action<DataverseEntitySetGetIn>? callBack = default)
    {
        var mock = new Mock<IDataverseEntitySetGetSupplier>();

        var m = mock.Setup(
            s => s.GetEntitySetAsync<CustomerSetFindJsonOut>(
                It.IsAny<DataverseEntitySetGetIn>(),
                It.IsAny<CancellationToken>()))
            .Returns(result.Pipe(ValueTask.FromResult));

        if (callBack is not null)
        {
            m.Callback<DataverseEntitySetGetIn, CancellationToken>(
                (@in, _) => callBack.Invoke(@in));
        }

        return mock;
    }
}
