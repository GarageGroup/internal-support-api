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
    private static IIncidentCreateFunc CreateFunc(IDataverseEntityCreateSupplier dataverseEntityCreateSupplier)
        =>
        Dependency.Of(dataverseEntityCreateSupplier)
        .UseIncidentCreateApi()
        .Resolve(Mock.Of<IServiceProvider>());

    private static Mock<IDataverseEntityCreateSupplier> CreateMockDataverseApiClient(
        Result<DataverseEntityCreateOut<CreateIncidentJsonOut>, Failure<int>> result, 
        Action<DataverseEntityCreateIn<CreateIncidentJsonIn>>? callBack = null)
    {
        var mock = new Mock<IDataverseEntityCreateSupplier>();
        
        var m = mock.Setup(
            s => s.CreateEntityAsync<CreateIncidentJsonIn, CreateIncidentJsonOut>(
                It.IsAny<DataverseEntityCreateIn<CreateIncidentJsonIn>>(), 
                It.IsAny<CancellationToken>()))
            .Returns(result.Pipe(ValueTask.FromResult));

        if(callBack is not null)
        {
            m.Callback<DataverseEntityCreateIn<CreateIncidentJsonIn>, CancellationToken>(
                (@in, _) => callBack.Invoke(@in));
        }

        return mock;
    }

    private static IncidentCreateIn SomeInput { get; } = 
        new(
            ownerId: Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            customerId: Guid.Parse("bd8b8e33-554e-e611-80dc-c4346bad0190"),
            title: "title",
            description: "decription",
            caseTypeCode: 1,
            caseOriginCode: 1);

    private const int DataverseNotFoundStatusCode = -2147220969;

    private const int DataversePicklistValueOutOfRangeStatusCode = -2147204326;
}
