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
    [InlineData(SomeContactId, IncidentCaseTypeCode.Question, IncidentPriorityCode.Low, "/contacts(" + SomeContactId + ")", 1, 3)]
    [InlineData(null, IncidentCaseTypeCode.Request, IncidentPriorityCode.Hight, null, 3, 1)]
    [InlineData(AnotherContactId, IncidentCaseTypeCode.Problem, IncidentPriorityCode.Normal, "/contacts(" + AnotherContactId + ")", 2, 2)]
    public async Task InvokeAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce(
        string? sourceContactId,
        IncidentCaseTypeCode sourceCaseTypeCode,
        IncidentPriorityCode sourcePriorityCode,
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
            priorityCode: sourcePriorityCode);

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