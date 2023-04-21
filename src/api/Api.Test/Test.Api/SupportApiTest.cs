using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using GGroupp.Infra;
using Moq;

namespace GGroupp.Internal.Support.Api.Test;

public static partial class SupportApiTest
{
    private static readonly IncidentCreateIn SomeIncidentCreateInput
        =
        new(
            ownerId: Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            customerId: Guid.Parse("bd8b8e33-554e-e611-80dc-c4346bad0190"),
            contactId: Guid.Parse("7a8b61a2-7f83-4519-9bd6-9a50c124613d"),
            title: "title",
            description: "decription",
            caseTypeCode: IncidentCaseTypeCode.Question,
            priorityCode: IncidentPriorityCode.Normal);

    private static readonly ContactSetSearchIn SomeContactSetSearchInput
        =
        new(
            searchText: "Some search text",
            customerId: Guid.Parse("3a727567-8e28-4e82-86fe-dbad1c74ef6f"))
        {
            Top = 5
        };

    private static readonly IncidentJsonCreateOut SomeIncidentJsonOutput
        =
        new()
        {
            IncidentId = Guid.Parse("ec8c8180-8ed7-4598-9bee-275262b396e2"),
            Title = "Some Incident title"
        };

    private static readonly DataverseSearchItem SomeDataverseContactItem
        =
        new(
            searchScore: 812634.97,
            objectId: Guid.Parse("2e96911b-1d7a-4258-b8a4-24c3c542d33b"),
            entityName: "Some Entity Name",
            extensionData: default);

    private static readonly DataverseSearchItem SomeDataverseCustomerItem
        =
        new(
            searchScore: 175.93,
            objectId: Guid.Parse("613e25fb-60bc-4b2b-be95-bddad08d4b14"),
            entityName: "Some entity",
            extensionData: default);

    private static readonly DataverseSearchItem SomeDataverseUserItem
        =
        new(
            searchScore: 251.71,
            objectId: Guid.Parse("5cc6ad62-9130-4f08-b69a-44f8b12247fd"),
            entityName: "Some entity",
            extensionData: new Dictionary<string, DataverseSearchJsonValue>
            {
                ["fullname"] = new(JsonSerializer.SerializeToElement("Some Name"))
            }.ToFlatArray());

    private static Mock<IFunc<Guid, Unit>> CreateMockImpersonateAction()
    {
        var mock = new Mock<IFunc<Guid, Unit>>();

        _ = mock.Setup(a => a.Invoke(It.IsAny<Guid>())).Returns(default(Unit));

        return mock;
    }

    private static Mock<IDataverseEntityCreateSupplier> CreateMockDataverseEntityCreateSupplier(
        Result<DataverseEntityCreateOut<IncidentJsonCreateOut>, Failure<DataverseFailureCode>> result)
    {
        var mock = new Mock<IDataverseEntityCreateSupplier>();
        
        _ = mock.Setup(
            s => s.CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>(
                It.IsAny<DataverseEntityCreateIn<IncidentJsonCreateIn>>(), 
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }

    private static Mock<IDataverseApiClient> CreateMockDataverseApiClient(
        Result<DataverseSearchOut, Failure<DataverseFailureCode>> result)
    {
        var mock = new Mock<IDataverseApiClient>();

        _ = mock
            .Setup(s => s.SearchAsync(It.IsAny<DataverseSearchIn>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(result);

        return mock;
    }

    private static Guid? ParseGuidOrNull(string? source)
        =>
        string.IsNullOrEmpty(source) ? null : Guid.Parse(source);
}