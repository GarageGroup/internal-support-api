using GGroupp.Infra;
using Moq;
using PrimeFuncPack;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GGroupp.Internal.Support.Incident.Create.Api.Tests;

using IIncidentCreateFunc = IAsyncValueFunc<IncidentCreateIn, Result<IncidentCreateOut, Failure<IncidentCreateFailureCode>>>;

public sealed partial class IncidentCreateFuncTest
{
    private const int DataverseNotFoundStatusCode = -2147220969;

    private const int DataversePicklistValueOutOfRangeStatusCode = -2147204326;

    private static readonly IncidentCreateIn SomeInput
        =
        new(
            ownerId: Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            customerId: Guid.Parse("bd8b8e33-554e-e611-80dc-c4346bad0190"),
            title: "title",
            description: "decription",
            caseTypeCode: 91,
            caseOriginCode: 7);

    private static IIncidentCreateFunc CreateFunc(IDataverseEntityCreateSupplier dataverseEntityCreateSupplier)
        =>
        Dependency.Of(dataverseEntityCreateSupplier)
        .UseIncidentCreateApi()
        .Resolve(Mock.Of<IServiceProvider>());

    private static Mock<IDataverseEntityCreateSupplier> CreateMockDataverseApiClient(
        Result<DataverseEntityCreateOut<IncidentJsonCreateOut>, Failure<int>> result, 
        Action<DataverseEntityCreateIn<IncidentJsonCreateIn>>? callback = null)
    {
        var mock = new Mock<IDataverseEntityCreateSupplier>();
        
        var m = mock.Setup(
            s => s.CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>(
                It.IsAny<DataverseEntityCreateIn<IncidentJsonCreateIn>>(), 
                It.IsAny<CancellationToken>()))
            .Returns(result.Pipe(ValueTask.FromResult));

        if(callback is not null)
        {
            m.Callback<DataverseEntityCreateIn<IncidentJsonCreateIn>, CancellationToken>(
                (@in, _) => callback.Invoke(@in));
        }

        return mock;
    }
}