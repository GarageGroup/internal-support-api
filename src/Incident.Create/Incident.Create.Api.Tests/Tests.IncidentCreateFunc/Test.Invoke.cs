using DeepEqual.Syntax;
using GGroupp.Infra;
using Moq;
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
    public async Task InvokeAsync_CancellationTokenHasCanceled_ExpectTaskCanceledException()
    {
        var success = new DataverseEntityCreateOut<CreateIncidentJsonOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);

        _ = await Assert.ThrowsAsync<TaskCanceledException>(() => func.InvokeAsync(Input, new CancellationToken(true)).AsTask());
    }

    [Fact]
    public async Task InvokeAsync_CancellationTokenHasNotCanceled_ExpectCallDataVerseApiClientOnce()
    {
        const string ownerId = "1203c0e2-3648-4596-80dd-127fdd2610b6";
        const string customerId = "bd8b8e33-554e-e611-80dc-c4346bad0190";
        const string title = "title";
        const string description = "description";

        var success = new DataverseEntityCreateOut<CreateIncidentJsonOut>(null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success, IsMatchDataverseInput);

        var token = new CancellationToken(false);
        var _input = new IncidentCreateIn(new(ownerId), new(customerId), title, description);

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
                        description: description));
            actual.ShouldDeepEqual(expected);
        }
    }

    [Theory]
    [InlineData(404, IncidentCreateFailureCode.Unknown)]
    [InlineData(int.MinValue, IncidentCreateFailureCode.Unknown)]
    [InlineData(int.MaxValue, IncidentCreateFailureCode.Unknown)]
    [InlineData(0, IncidentCreateFailureCode.Unknown)]
    [InlineData(DataverseNotFoundStatusCode, IncidentCreateFailureCode.NotFound)]
    public async Task InvokeAsync_FailureResultIsGiven_ExpectFailure(int failureCode, IncidentCreateFailureCode incidentCreateFailureCode)
    {
        const string failureMessge = "Bad request";
        var failure = Failure.Create(failureCode, failureMessge);
        var mockDataverseApiClient = CreateMockDataverseApiClient(failure);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(Input, CancellationToken.None);

        var expectedFailure = Failure.Create(incidentCreateFailureCode, failureMessge);
        Assert.Equal(expectedFailure, actualResult);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsGiven_ExpectSuccessResult()
    {
        Guid incidentId = Guid.Parse("1203c0e2-3648-4596-80dd-127fdd2610b6");
        var title = "title";

        var success = new DataverseEntityCreateOut<CreateIncidentJsonOut>(new(incidentId, title));
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(Input, default);

        var expectedFailure = new IncidentCreateOut(incidentId, title);
        Assert.Equal(expectedFailure, actualResult);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsNull_ExpectSuccessResult()
    {
        var success = new DataverseEntityCreateOut<CreateIncidentJsonOut>(null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(Input, default);

        var expected = new IncidentCreateOut(default, default);
        Assert.Equal(expected, actualResult);
    }
}
