using GGroupp.Infra;
using Moq;
using PrimeFuncPack;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support.CustomerSet.Search.Api.Tests;

using ICustomerSetSearchFunc = IAsyncValueFunc<CustomerSetFindIn, Result<CustomerSetFindOut, Failure<CustomerSetFindFailureCode>>>;

public sealed partial class CustomerSetSearchFuncTest
{
    private static ICustomerSetSearchFunc CreateFunc(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        Dependency.Of(dataverseSearchSupplier)
        .UseCustomerSetSearchApi()
        .Resolve(Mock.Of<IServiceProvider>());

    private static Mock<IDataverseSearchSupplier> CreateMockDataverseApiClient(
        Result<DataverseSearchOut, Failure<int>> result,
        Action<DataverseSearchIn>? callBack = default)
    {
        var mock = new Mock<IDataverseSearchSupplier>();

        var m = mock.Setup(
            s => s.SearchAsync(
                It.IsAny<DataverseSearchIn>(),
                It.IsAny<CancellationToken>()))
            .Returns(result.Pipe(ValueTask.FromResult));

        if (callBack is not null)
        {
            m.Callback<DataverseSearchIn, CancellationToken>(
                (@in, _) => callBack.Invoke(@in));
        }

        return mock;
    }
}
