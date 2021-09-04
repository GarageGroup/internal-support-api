using DeepEqual.Syntax;
using GGroupp.Infra;
using Moq;
using Xunit;

namespace GGroupp.Internal.Support.CustomerSet.Find.Api.Tests;

partial class CustomerSetFindFuncTest
{
    [Fact]
    public async Task InvokeAsync_InputIsNull_ExpectArgumentNullExcepcion()
    {
        var success = new DataverseEntitySetGetOut<CustomerSetFindJsonOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => func.InvokeAsync(null!, CancellationToken.None).AsTask());
        Assert.Equal("input", exception.ParamName);
    }

    [Fact]
    public async Task InvokeAsync_CancellationTokenHasCanceled_ExpectTaskCanceledException()
    {
        var success = new DataverseEntitySetGetOut<CustomerSetFindJsonOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);

        _ = await Assert.ThrowsAsync<TaskCanceledException>(() => func.InvokeAsync(new(""), new CancellationToken(true)).AsTask());
    }

    [Fact]
    public async  Task InvokeAsync_CancellationTokenHasNotCanceled_ExpectCallDataVerseApiClientOnce()
    {
        const string searchString = "льс";
        

        var success = new DataverseEntitySetGetOut<CustomerSetFindJsonOut>(null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success, IsMatchDataverseInput);

        var token = new CancellationToken(false);
        

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(new(searchString), token);

        mockDataverseApiClient.Verify(
            c => c.GetEntitySetAsync<CustomerSetFindJsonOut>(
                It.IsAny<DataverseEntitySetGetIn>(), token), 
            Times.Once);

        static void IsMatchDataverseInput(DataverseEntitySetGetIn actual)
        {
            var expected = new DataverseEntitySetGetIn(
                entitySetName: "accounts",
                selectFields: new[] { "name", "accountid" },
                filter: $"contains(name,'{searchString}')");
            actual.ShouldDeepEqual(expected);
        }
    }

    [Theory]
    [InlineData(404)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(0)]
    [InlineData(-2147220969)]
    public async Task InvokeAsync_FailureResultIsGiven_ExpectFailure(int failureCode)
    {
        const string failureMessge = "Bad request";
        var failure = Failure.Create(failureCode, failureMessge);
        var mockDataverseApiClient = CreateMockDataverseApiClient(failure);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(new(""), CancellationToken.None);

        var expectedFailure = Failure.Create(CustomerSetFindFailureCode.Unknown, failureMessge);
        Assert.Equal(expectedFailure, actualResult);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsGiven_ExpectSuccessResult()
    {
        
        var searchText = "title";
        var accountId = Guid.NewGuid();
        var title = "Renesans";

        var success = new DataverseEntitySetGetOut<CustomerSetFindJsonOut>( new CustomerSetFindJsonOut[] { new(accountId, title) });
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = (await func.InvokeAsync(new(searchText), default)).SuccessOrThrow();

        var expectedFailure = new CustomerSetFindOut(new CustomerItemFindOut[] { new(accountId, title) });
        Assert.Equal(expectedFailure.Customers, actualResult.Customers);
    }

    [Fact]
    public async  Task InvokeAsync_SuccessResultIsEmptyArray_ExpectSuccessResult()
    {
        var success = new DataverseEntitySetGetOut<CustomerSetFindJsonOut>(new CustomerSetFindJsonOut[0]);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(new(""), default);

        var expected = new CustomerSetFindOut(new CustomerItemFindOut[0]);
        Assert.Equal(expected.Customers, actualResult.SuccessOrThrow().Customers);
    }
}
