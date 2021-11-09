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
    public async Task InvokeAsync_InputIsNull_ExpectArgumentNullExcepcion()
    {
        var success = new DataverseEntityCreateOut<CreateIncidentJsonOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => func.InvokeAsync(null!, CancellationToken.None).AsTask());
        Assert.Equal("input", exception.ParamName);
    }

    [Fact]
    public void InvokeAsync_CancellationTokenIsCanceled_ExpectValueTaskIsCanceled()
    {
        var success = new DataverseEntityCreateOut<CreateIncidentJsonOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = SomeInput;
        var token = new CancellationToken(canceled: true);

        var actual = func.InvokeAsync(input, token);
        Assert.True(actual.IsCanceled);
    }

    [Fact]
    public async Task InvokeAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce()
    {
        const string ownerId = "1203c0e2-3648-4596-80dd-127fdd2610b6";
        const string customerId = "bd8b8e33-554e-e611-80dc-c4346bad0190";
        const string title = "title";
        const string description = "description";
        const int caseTypeCode = 1;
        const int caseOriginCode = 1;

        var success = new DataverseEntityCreateOut<CreateIncidentJsonOut>(null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success, IsMatchDataverseInput);

        var token = new CancellationToken(false);
        var _input = new IncidentCreateIn(new(ownerId), new(customerId), title, description, caseTypeCode, caseOriginCode);

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(_input, token);

        mockDataverseApiClient.Verify(
            c => c.CreateEntityAsync<CreateIncidentJsonIn, CreateIncidentJsonOut>(
                It.IsAny<DataverseEntityCreateIn<CreateIncidentJsonIn>>(), token), 
            Times.Once);

        static void IsMatchDataverseInput(DataverseEntityCreateIn<CreateIncidentJsonIn> actual)
        {
            var expected = new DataverseEntityCreateIn<CreateIncidentJsonIn>(
                    entityPluralName: "incidents",
                    selectFields: new[] { "incidentid", "title" },
                    entityData: new(
                        ownerId: $"/systemusers({ownerId})",
                        customerId: $"/accounts({customerId})",
                        title: title,
                        description: description,
                        caseTypeCode: caseTypeCode,
                        caseOriginCode: caseOriginCode));
            actual.ShouldDeepEqual(expected);
        }
    }

    [Theory]
    [InlineData(404, IncidentCreateFailureCode.Unknown)]
    [InlineData(int.MinValue, IncidentCreateFailureCode.Unknown)]
    [InlineData(int.MaxValue, IncidentCreateFailureCode.Unknown)]
    [InlineData(0, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseNotFoundStatusCode, IncidentCreateFailureCode.NotFound)]
    [InlineData(DataversePicklistValueOutOfRangeStatusCode, IncidentCreateFailureCode.UnexpectedCaseCode)]
    public async Task InvokeAsync_FailureResultIsGiven_ExpectFailure(int failureCode, IncidentCreateFailureCode incidentCreateFailureCode)
    {
        const string failureMessge = "Bad request";
        var failure = Failure.Create(failureCode, failureMessge);
        var mockDataverseApiClient = CreateMockDataverseApiClient(failure);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(SomeInput, CancellationToken.None);

        var expectedFailure = Failure.Create(incidentCreateFailureCode, failureMessge);
        Assert.Equal(expectedFailure, actualResult);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsGiven_ExpectSuccessResult()
    {
        Guid incidentId = Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6");
        var title = "title";

        var success = new DataverseEntityCreateOut<CreateIncidentJsonOut>(new() { IncidentId = incidentId, Title = title});
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(SomeInput, default);

        var expectedSuccess = new IncidentCreateOut(incidentId, title);
        Assert.Equal(expectedSuccess, actualResult);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsNull_ExpectSuccessResult()
    {
        var success = new DataverseEntityCreateOut<CreateIncidentJsonOut>(null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(SomeInput, default);

        var expected = new IncidentCreateOut(default, default);
        Assert.Equal(expected, actualResult);
    }
}
