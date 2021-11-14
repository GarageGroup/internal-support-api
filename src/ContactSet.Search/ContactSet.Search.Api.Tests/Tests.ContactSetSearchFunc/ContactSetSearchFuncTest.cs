using GGroupp.Infra;
using Moq;
using PrimeFuncPack;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support.ContactSet.Search.Api.Tests;

using IContactSetSearchFunc = IAsyncValueFunc<ContactSetSearchIn, Result<ContactSetSearchOut, Failure<ContactSetSearchFailureCode>>>;

public sealed partial class ContactSetSearchFuncTest
{
    private static readonly DataverseSearchItem SomeDataverseSearchItem
        =
        new(
            searchScore: 812634.97,
            objectId: Guid.Parse("2e96911b-1d7a-4258-b8a4-24c3c542d33b"),
            entityName: "Some Entity Name",
            extensionData: default);

    private static readonly ContactSetSearchIn SomeInput
        =
        new(
            searchText: "Some search text",
            customerId: Guid.Parse("3a727567-8e28-4e82-86fe-dbad1c74ef6f"),
            top: 5);

    private static IContactSetSearchFunc CreateFunc(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        Dependency.Of(dataverseSearchSupplier)
        .UseContactSetSearchApi()
        .Resolve(Mock.Of<IServiceProvider>());

    private static Mock<IDataverseSearchSupplier> CreateMockDataverseApiClient(
        Result<DataverseSearchOut, Failure<int>> result,
        Action<DataverseSearchIn>? callBack = default)
    {
        var mock = new Mock<IDataverseSearchSupplier>();

        var m = mock
            .Setup(s => s.SearchAsync(It.IsAny<DataverseSearchIn>(), It.IsAny<CancellationToken>()))
            .Returns(result.Pipe(ValueTask.FromResult));

        if (callBack is not null)
        {
            m.Callback<DataverseSearchIn, CancellationToken>(
                (@in, _) => callBack.Invoke(@in));
        }

        return mock;
    }
}