using System;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Support.Api.Test;

partial class SupportApiTest
{
    [Fact]
    public static async Task CreateIncidentAsync_InputIsNull_ExpectArgumentNullException()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);
        var mockEntityCreateSupplier = CreateMockDataverseEntityCreateSupplier(dataverseOut);

        var dataverseApiClient = new StubDataverseApiClient(mockEntityCreateSupplier.Object);
        var api = new SupportApi(dataverseApiClient);

        var cancellationToken = new CancellationToken(canceled: false);

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(TestAsync);
        Assert.Equal("input", ex.ParamName);

        async Task TestAsync()
            =>
            _ = await api.CreateIncidentAsync(null!, cancellationToken);
    }

    [Fact]
    public static void CreateIncidentAsync_CancellationTokenIsCanceled_ExpectValueTaskIsCanceled()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);
        var mockEntityCreateSupplier = CreateMockDataverseEntityCreateSupplier(dataverseOut);

        var dataverseApiClient = new StubDataverseApiClient(mockEntityCreateSupplier.Object);
        var api = new SupportApi(dataverseApiClient);

        var cancellationToken = new CancellationToken(canceled: true);
        var actual = api.CreateIncidentAsync(SomeIncidentCreateInput, cancellationToken);

        Assert.True(actual.IsCanceled);
    }

    [Theory]
    [InlineData(
        "be761c38-5d95-47c2-b4aa-1056e61a1cb0",
        IncidentCaseTypeCode.Question,
        IncidentPriorityCode.Low,
        "8d690bea-2c1d-4ded-b5c2-0d070e8559f1",
        "/contacts(be761c38-5d95-47c2-b4aa-1056e61a1cb0)",
        1,
        3)]
    [InlineData(
        null,
        IncidentCaseTypeCode.Request,
        IncidentPriorityCode.Hight,
        "144326e7-5aa8-4792-b8aa-d125b6f7f6b5",
        null,
        3,
        1)]
    [InlineData(
        "5d22feb3-b450-4129-8bfb-729043042dfa",
        IncidentCaseTypeCode.Problem,
        IncidentPriorityCode.Normal,
        null,
        "/contacts(5d22feb3-b450-4129-8bfb-729043042dfa)",
        2,
        2)]
    public static async Task CreateIncidentAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce(
        string? sourceContactId,
        IncidentCaseTypeCode sourceCaseTypeCode,
        IncidentPriorityCode sourcePriorityCode,
        string? callerUserId,
        string? expectedContactId,
        int expectedCaseTypeCode,
        int expectedPriorityCode)
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);
        var mockEntityCreateSupplier = CreateMockDataverseEntityCreateSupplier(dataverseOut);

        var dataverseApiClient = new StubDataverseApiClient(mockEntityCreateSupplier.Object);
        var api = new SupportApi(dataverseApiClient);

        var input = new IncidentCreateIn(
            ownerId: Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            customerId: Guid.Parse("bd8b8e33-554e-e611-80dc-c4346bad0190"),
            contactId: ParseGuidOrNull(sourceContactId),
            title: "Some title",
            description: "Some description",
            caseTypeCode: sourceCaseTypeCode,
            priorityCode: sourcePriorityCode)
        {
            CallerUserId = ParseGuidOrNull(callerUserId)
        };

        var token = new CancellationToken(false);
        _ = await api.CreateIncidentAsync(input, token);

        var expected = new DataverseEntityCreateIn<IncidentJsonCreateIn>(
                entityPluralName: "incidents",
                selectFields: new("incidentid", "title"),
                entityData: new()
                {
                    OwnerId = "/systemusers(1203c0e2-3648-4596-80dd-127fdd2610b6)",
                    CustomerId = "/accounts(bd8b8e33-554e-e611-80dc-c4346bad0190)",
                    ContactId = expectedContactId,
                    Title = "Some title",
                    Description = "Some description",
                    CaseTypeCode = expectedCaseTypeCode,
                    PriorityCode = expectedPriorityCode,
                    CaseOriginCode = null
                });

        mockEntityCreateSupplier.Verify(
            c => c.CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>(expected, token), Times.Once);
    }

    [Fact]
    public static async Task CreateIncidentAsync_CallerUserIdIsNull_ExpectCallImpersonateNever()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);
        var mockEntityCreateSupplier = CreateMockDataverseEntityCreateSupplier(dataverseOut);

        var mockImpersonateAction = CreateMockImpersonateAction();
        var dataverseApiClient = new StubDataverseApiClient(mockEntityCreateSupplier.Object, mockImpersonateAction.Object);

        var api = new SupportApi(dataverseApiClient);

        var input = new IncidentCreateIn(
            ownerId: Guid.Parse("041eb1fd-c185-4e17-9ce3-7bb754ce84b6"),
            customerId: Guid.Parse("b3a2b17c-3c49-4c58-b365-3ff6dc168b6d"),
            contactId: Guid.Parse("3340639e-847c-49e0-9bad-ee05a8ea0a0f"),
            title: "Some title",
            description: "Some description",
            caseTypeCode: IncidentCaseTypeCode.Problem,
            priorityCode: IncidentPriorityCode.Hight);

        _ = await api.CreateIncidentAsync(input, default);
        mockImpersonateAction.Verify(a => a.Invoke(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public static async Task CreateIncidentAsync_CallerUserIdIsNotNull_ExpectCallImpersonateOnce()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(SomeIncidentJsonOutput);
        var mockEntityCreateSupplier = CreateMockDataverseEntityCreateSupplier(dataverseOut);

        var mockImpersonateAction = CreateMockImpersonateAction();
        var dataverseApiClient = new StubDataverseApiClient(mockEntityCreateSupplier.Object, mockImpersonateAction.Object);

        var api = new SupportApi(dataverseApiClient);

        var input = new IncidentCreateIn(
            ownerId: Guid.Parse("0c1040cc-6dff-4eda-b40b-38b04b72bb82"),
            customerId: Guid.Parse("4b4d6147-da68-4dea-b8da-f0090d118b12"),
            contactId: Guid.Parse("6cdae1fe-f0c8-4664-9c0c-579aae1ce242"),
            title: "Some title",
            description: "Some description",
            caseTypeCode: IncidentCaseTypeCode.Problem,
            priorityCode: IncidentPriorityCode.Hight)
        {
            CallerUserId = Guid.Parse("de42801c-ae9b-4be1-bd39-a0a70324539f")
        };

        _ = await api.CreateIncidentAsync(input, default);
        mockImpersonateAction.Verify(a => a.Invoke(Guid.Parse("de42801c-ae9b-4be1-bd39-a0a70324539f")), Times.Once);
    }

    [Theory]
    [InlineData(DataverseFailureCode.Unknown, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.DuplicateRecord, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, IncidentCreateFailureCode.NotFound)]
    [InlineData(DataverseFailureCode.PrivilegeDenied, IncidentCreateFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.UserNotEnabled, IncidentCreateFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.Throttling, IncidentCreateFailureCode.TooManyRequests)]
    public static async Task CreateIncidentAsync_DataverseResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, IncidentCreateFailureCode expectedFailureCode)
    {
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some failure message");
        var mockEntityCreateSupplier = CreateMockDataverseEntityCreateSupplier(dataverseFailure);

        var dataverseApiClient = new StubDataverseApiClient(mockEntityCreateSupplier.Object);
        var api = new SupportApi(dataverseApiClient);

        var actual = await api.CreateIncidentAsync(SomeIncidentCreateInput, CancellationToken.None);
        var expected = Failure.Create(expectedFailureCode, dataverseFailure.FailureMessage);

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task CreateIncidentAsync_DataverseResultIsSuccess_ExpectSuccess()
    {
        var incidentJsonOut = new IncidentJsonCreateOut
        {
            IncidentId = Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            Title = "Some incident title"
        };

        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(incidentJsonOut);
        var mockEntityCreateSupplier = CreateMockDataverseEntityCreateSupplier(dataverseOut);

        var dataverseApiClient = new StubDataverseApiClient(mockEntityCreateSupplier.Object);
        var api = new SupportApi(dataverseApiClient);

        var actual = await api.CreateIncidentAsync(SomeIncidentCreateInput, default);
        var expected = new IncidentCreateOut(incidentJsonOut.IncidentId, incidentJsonOut.Title);

        Assert.StrictEqual(expected, actual);
    }
}