using DeepEqual.Syntax;
using GGroupp.Infra;
using Moq;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GGroupp.Internal.Support.Incident.Create.Api.Tests;

partial class IncidentCreateFuncTest
{
    [Fact]
    public void InvokeAsync_CancellationTokenIsCanceled_ExpectValueTaskIsCanceled()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = SomeInput;
        var token = new CancellationToken(canceled: true);

        var actual = func.InvokeAsync(input, token);
        Assert.True(actual.IsCanceled);
    }

    [Theory]
    [InlineData(SomeContactId, IncidentCaseTypeCode.Question, IncidentPriorityCode.Low, SomeUserId, "/contacts(" + SomeContactId + ")", 1, 3)]
    [InlineData(null, IncidentCaseTypeCode.Request, IncidentPriorityCode.Hight, AnotherUserId, null, 3, 1)]
    [InlineData(AnotherContactId, IncidentCaseTypeCode.Problem, IncidentPriorityCode.Normal, null, "/contacts(" + AnotherContactId + ")", 2, 2)]
    public async Task InvokeAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce(
        string? sourceContactId,
        IncidentCaseTypeCode sourceCaseTypeCode,
        IncidentPriorityCode sourcePriorityCode,
        string? callerUserId,
        string? expectedContactId,
        int expectedCaseTypeCode,
        int expectedPriorityCode)
    {
        const string ownerId = "1203c0e2-3648-4596-80dd-127fdd2610b6";
        const string customerId = "bd8b8e33-554e-e611-80dc-c4346bad0190";

        const string title = "Some title";
        const string description = "Some description";

        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut, IsMatchDataverseInput);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new IncidentCreateIn(
            ownerId: Guid.Parse(ownerId),
            customerId: Guid.Parse(customerId),
            contactId: sourceContactId is not null ? Guid.Parse(sourceContactId) : null,
            title: title,
            description: description,
            caseTypeCode: sourceCaseTypeCode,
            priorityCode: sourcePriorityCode,
            callerUserId: callerUserId is not null ? Guid.Parse(callerUserId) : null);

        var token = new CancellationToken(false);
        _ = await func.InvokeAsync(input, token);

        mockDataverseApiClient.Verify(
            c => c.CreateEntityAsync<IncidentJsonCreateIn, IncidentJsonCreateOut>(
                It.IsAny<DataverseEntityCreateIn<IncidentJsonCreateIn>>(), token),
            Times.Once);

        void IsMatchDataverseInput(DataverseEntityCreateIn<IncidentJsonCreateIn> actual)
        {
            var expected = new DataverseEntityCreateIn<IncidentJsonCreateIn>(
                entityPluralName: "incidents",
                selectFields: new[] { "incidentid", "title" },
                entityData: new(
                    ownerId: $"/systemusers({ownerId})",
                    customerId: $"/accounts({customerId})",
                    contactId: expectedContactId,
                    title: title,
                    description: description,
                    caseTypeCode: expectedCaseTypeCode,
                    priorityCode: expectedPriorityCode,
                    caseOriginCode: null));

            actual.ShouldDeepEqual(expected);
        }
    }

    [Fact]
    public async Task InvokeAsync_CallerUserIdIsNull_ExpectCallImpersonateNever()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var mockImpersonateAction = CreateMockImpersonateAction();
        var func = CreateFunc(mockDataverseApiClient.Object, mockImpersonateAction.Object);

        var input = new IncidentCreateIn(
            ownerId: Guid.Parse("041eb1fd-c185-4e17-9ce3-7bb754ce84b6"),
            customerId: Guid.Parse("b3a2b17c-3c49-4c58-b365-3ff6dc168b6d"),
            contactId: Guid.Parse("3340639e-847c-49e0-9bad-ee05a8ea0a0f"),
            title: "Some title",
            description: "Some description",
            caseTypeCode: IncidentCaseTypeCode.Problem,
            priorityCode: IncidentPriorityCode.Hight,
            callerUserId: null);

        _ = await func.InvokeAsync(input, default);
        mockImpersonateAction.Verify(a => a.Invoke(It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task InvokeAsync_CallerUserIdIsNotNull_ExpectCallImpersonateOnce()
    {
        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var mockImpersonateAction = CreateMockImpersonateAction();
        var func = CreateFunc(mockDataverseApiClient.Object, mockImpersonateAction.Object);

        var callerUserId = Guid.Parse("de42801c-ae9b-4be1-bd39-a0a70324539f");

        var input = new IncidentCreateIn(
            ownerId: Guid.Parse("0c1040cc-6dff-4eda-b40b-38b04b72bb82"),
            customerId: Guid.Parse("4b4d6147-da68-4dea-b8da-f0090d118b12"),
            contactId: Guid.Parse("6cdae1fe-f0c8-4664-9c0c-579aae1ce242"),
            title: "Some title",
            description: "Some description",
            caseTypeCode: IncidentCaseTypeCode.Problem,
            priorityCode: IncidentPriorityCode.Hight,
            callerUserId: callerUserId);

        _ = await func.InvokeAsync(input, default);
        mockImpersonateAction.Verify(a => a.Invoke(callerUserId), Times.Once);
    }

    [Theory]
    [InlineData(DataverseFailureCode.Unknown, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, IncidentCreateFailureCode.NotFound)]
    [InlineData(DataverseFailureCode.PrivilegeDenied, IncidentCreateFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.UserNotEnabled, IncidentCreateFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.Throttling, IncidentCreateFailureCode.TooManyRequests)]
    public async Task InvokeAsync_DataverseResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, IncidentCreateFailureCode expectedFailureCode)
    {
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some failure message");
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseFailure);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actual = await func.InvokeAsync(SomeInput, CancellationToken.None);

        var expected = Failure.Create(expectedFailureCode, dataverseFailure.FailureMessage);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task InvokeAsync_DataverseResultIsSuccess_ExpectSuccess()
    {
        var incidentJsonOut = new IncidentJsonCreateOut
        {
            IncidentId = Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6"),
            Title = "Some incident title"
        };

        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(incidentJsonOut);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actual = await func.InvokeAsync(SomeInput, default);

        var expected = new IncidentCreateOut(incidentJsonOut.IncidentId, incidentJsonOut.Title);
        Assert.Equal(expected, actual);
    }
}