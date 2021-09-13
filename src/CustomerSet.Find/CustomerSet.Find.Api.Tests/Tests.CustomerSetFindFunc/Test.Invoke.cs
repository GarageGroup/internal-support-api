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
    public void InvokeAsync_CancellationTokenIsCanceled_ExpectTaskIsCanceled()
    {
        var success = new DataverseEntitySetGetOut<CustomerSetFindJsonOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var valueTask = func.InvokeAsync(new(string.Empty), new CancellationToken(true));
        Assert.True(valueTask.IsCanceled);
    }

    [Theory]
    [InlineData("\u043B\u044C\u0441", "contains(name,'\u043B\u044C\u0441')")]
    [InlineData(Strings.Empty, Strings.Empty)]
    [InlineData(null, Strings.Empty)]
    public async Task InvokeAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce(
        string searchString, string expectedFilter)
    {
        var success = new DataverseEntitySetGetOut<CustomerSetFindJsonOut>(null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success, IsMatchDataverseInput);

        var token = new CancellationToken(false);

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(new(searchString), token);

        mockDataverseApiClient.Verify(
            c => c.GetEntitySetAsync<CustomerSetFindJsonOut>(
                It.IsAny<DataverseEntitySetGetIn>(), token),
            Times.Once);

        void IsMatchDataverseInput(DataverseEntitySetGetIn actual)
        {
            var expected = new DataverseEntitySetGetIn(
                entitySetName: "accounts",
                selectFields: new[] { "name", "accountid" },
                filter: expectedFilter);
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
        var actualResult = await func.InvokeAsync(new(string.Empty), CancellationToken.None);

        var expectedFailure = Failure.Create(CustomerSetFindFailureCode.Unknown, failureMessge);
        Assert.Equal(expectedFailure, actualResult);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsGiven_ExpectSuccessResult()
    {
        var searchText = "title";
        var accountId = Guid.Parse("1b91d06f-208d-4c1c-b630-0ee9996a8a59");
        var title = "Renessans";

        var success = new DataverseEntitySetGetOut<CustomerSetFindJsonOut>(new CustomerSetFindJsonOut[] { new(accountId, title) });
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = (await func.InvokeAsync(new(searchText), default));
        Assert.True(actualResult.IsSuccess);

        var actual = actualResult.SuccessOrThrow().Customers;
        var expected = new CustomerItemFindOut[] { new(accountId, title) };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsEmptyArray_ExpectSuccessResult()
    {
        var success = new DataverseEntitySetGetOut<CustomerSetFindJsonOut>(new CustomerSetFindJsonOut[0]);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(new(string.Empty), default);

        Assert.True(actualResult.IsSuccess);
        var customers = actualResult.SuccessOrThrow().Customers;

        Assert.Empty(customers);
    }
}
