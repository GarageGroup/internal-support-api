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
    private const string SomeContactId = "b6010aeb-bd3c-ec11-b6e5-000d3abfc6af";

    private const string AnotherContactId = "c4934709-0ab1-4f20-8168-13140033caf6";

    private const string SomeUserId = "a155bb95-b509-4f12-824e-0c3024c3c692";

    private const string AnotherUserId = "1962491f-aaa1-4464-ba79-b231896f3070";

    private static readonly IncidentCreateIn SomeInput
        =
        new(
            ownerId: Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            customerId: Guid.Parse("bd8b8e33-554e-e611-80dc-c4346bad0190"),
            contactId: Guid.Parse("7a8b61a2-7f83-4519-9bd6-9a50c124613d"),
            title: "title",
            description: "decription",
            caseTypeCode: IncidentCaseTypeCode.Question,
            priorityCode: IncidentPriorityCode.Normal);

    private static IIncidentCreateFunc CreateFunc(IDataverseEntityCreateSupplier dataverseEntityCreateSupplier)
        =>
        Dependency.Of(dataverseEntityCreateSupplier)
        .Map(StubDataverseApiClient.Create)
        .UseIncidentCreateApi()
        .Resolve(Mock.Of<IServiceProvider>());

    private static IIncidentCreateFunc CreateFunc(IDataverseEntityCreateSupplier dataverseEntityCreateSupplier, IFunc<Guid, Unit> impersonateAction)
        =>
        Dependency.Of(dataverseEntityCreateSupplier, impersonateAction)
        .Fold(StubDataverseApiClient.Create)
        .UseIncidentCreateApi()
        .Resolve(Mock.Of<IServiceProvider>());

    private static Mock<IDataverseEntityCreateSupplier> CreateMockDataverseApiClient(
        Result<DataverseEntityCreateOut<IncidentJsonCreateOut>, Failure<DataverseFailureCode>> result)
    {
        var mock = new Mock<IDataverseEntityCreateSupplier>();
        
        _ = mock.Setup(
            s => s.CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>(
                It.IsAny<DataverseEntityCreateIn<IncidentJsonCreateIn>>(), 
                It.IsAny<CancellationToken>()))
            .Returns(new ValueTask<Result<DataverseEntityCreateOut<IncidentJsonCreateOut>, Failure<DataverseFailureCode>>>(result));

        return mock;
    }

    private static Mock<IFunc<Guid, Unit>> CreateMockImpersonateAction()
    {
        var mock = new Mock<IFunc<Guid, Unit>>();

        _ = mock.Setup(a => a.Invoke(It.IsAny<Guid>())).Returns(default(Unit));

        return mock;
    }
}