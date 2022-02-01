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
    [InlineData("b6010aeb-bd3c-ec11-b6e5-000d3abfc6af", "/contacts(b6010aeb-bd3c-ec11-b6e5-000d3abfc6af)")]
    [InlineData(null, null)]
    public async Task InvokeAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce(string? sourceContactId, string? expectedContactId)
    {
        const string ownerId = "1203c0e2-3648-4596-80dd-127fdd2610b6";
        const string customerId = "bd8b8e33-554e-e611-80dc-c4346bad0190";

        const string title = "Some title";
        const string description = "Some description";

        const int caseTypeCode = 357;
        const int caseOriginCode = 12358;

        var dataverseOut = new DataverseEntityCreateOut<IncidentJsonCreateOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut, IsMatchDataverseInput);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new IncidentCreateIn(
            ownerId: Guid.Parse(ownerId),
            customerId: Guid.Parse(customerId),
            title: title,
            description: description,
            caseTypeCode: caseTypeCode,
            caseOriginCode: caseOriginCode,
            contactId: sourceContactId is not null ? Guid.Parse(sourceContactId) : null);

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
                    title: title,
                    description: description,
                    caseTypeCode: caseTypeCode,
                    caseOriginCode: caseOriginCode,
                    contactId: expectedContactId));

            actual.ShouldDeepEqual(expected);
        }
    }

    [Theory]
    [InlineData(DataverseFailureCode.Unknown, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, IncidentCreateFailureCode.NotFound)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, IncidentCreateFailureCode.UnexpectedCaseCode)]
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