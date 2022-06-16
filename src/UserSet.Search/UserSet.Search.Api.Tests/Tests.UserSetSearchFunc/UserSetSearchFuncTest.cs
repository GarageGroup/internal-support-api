using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using Moq;
using PrimeFuncPack;

namespace GGroupp.Internal.Support.UserSet.Search.Api.Testss;

using IUserSetSearchFunc = IAsyncValueFunc<UserSetSearchIn, Result<UserSetSearchOut, Failure<UserSetSearchFailureCode>>>;

public sealed partial class UserSetSearchFuncTest
{
    private static readonly DataverseSearchItem SomeDataverseSearchItem
        =
        new(
            searchScore: 251.71,
            objectId: Guid.Parse("5cc6ad62-9130-4f08-b69a-44f8b12247fd"),
            entityName: "Some entity",
            extensionData: new Dictionary<string, DataverseSearchJsonValue>
            {
                ["fullname"] = new(JsonSerializer.SerializeToElement("Some Name"))
            });

    private static IUserSetSearchFunc CreateFunc(IDataverseSearchSupplier dataverseSearchSupplier)
        =>
        Dependency.Of(dataverseSearchSupplier).UseUserSetSearchApi().Resolve(Mock.Of<IServiceProvider>());

    private static Mock<IDataverseSearchSupplier> CreateMockDataverseApiClient(
        Result<DataverseSearchOut, Failure<DataverseFailureCode>> result,
        Action<DataverseSearchIn>? callBack = default)
    {
        var mock = new Mock<IDataverseSearchSupplier>();

        var m = mock
            .Setup(s => s.SearchAsync(It.IsAny<DataverseSearchIn>(), It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<Result<DataverseSearchOut, Failure<DataverseFailureCode>>>(result));

        if (callBack is not null)
        {
            m.Callback<DataverseSearchIn, CancellationToken>(
                (@in, _) => callBack.Invoke(@in));
        }

        return mock;
    }
}