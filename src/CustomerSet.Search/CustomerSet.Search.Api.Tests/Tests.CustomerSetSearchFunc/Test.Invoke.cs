using DeepEqual.Syntax;
using GGroupp.Infra;
using Moq;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GGroupp.Internal.Support.CustomerSet.Search.Api.Tests;

partial class CustomerSetSearchFuncTest
{
    [Fact]
    public void InvokeAsync_CancellationTokenIsCanceled_ExpectTaskIsCanceled()
    {
        var dataverseOut = new DataverseSearchOut(51, new[] { SomeDataverseSearchItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn("Some search text");
        var token = new CancellationToken(canceled: true);

        var actualTask = func.InvokeAsync(input, token);
        Assert.True(actualTask.IsCanceled);
    }

    [Theory]
    [InlineData("\u043B\u044C\u0441")]
    [InlineData(Strings.Empty)]
    [InlineData(null)]
    public async Task InvokeAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce(string? searchString)
    {
        var dataverseOut = new DataverseSearchOut(17, new[] { SomeDataverseSearchItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut, IsMatchDataverseInput);

        var input = new CustomerSetSearchIn(searchString);
        var token = new CancellationToken(canceled: false);

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(input, token);

        mockDataverseApiClient.Verify(c => c.SearchAsync(It.IsAny<DataverseSearchIn>(), token), Times.Once);

        void IsMatchDataverseInput(DataverseSearchIn actual)
        {
            var expected = new DataverseSearchIn($"*{searchString}*")
            {
                Entities = new[] { "account" }
            };
            actual.ShouldDeepEqual(expected);
        }
    }

    [Theory]
    [InlineData(404)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(0)]
    [InlineData(-2147220969)]
    public async Task InvokeAsync_DataverseSearchResultIsFailure_ExpectFailure(int failureCode)
    {
        const string failureMessge = "Some failure message";
        var dataverseFailure = Failure.Create(failureCode, failureMessge);

        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseFailure);
        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn("Some search text", 5);
        var actual = await func.InvokeAsync(input, CancellationToken.None);

        var expected = Failure.Create(CustomerSetSearchFailureCode.Unknown, failureMessge);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task InvokeAsync_DataverseSearchResultIsSuccessWithName_ExpectSuccess()
    {
        var searchText = "Some Search Text";
        var accountId = Guid.Parse("1b91d06f-208d-4c1c-b630-0ee9996a8a59");
        var title = "Some title";

        var jsonElement = JsonSerializer.SerializeToElement(title);

        var searchItem = new DataverseSearchItem(
            searchScore: -917.095,
            objectId: accountId,
            entityName: "Some entity name",
            extensionData: new Dictionary<string, DataverseSearchJsonValue>
            {
                ["name"] = new(jsonElement)
            });

        var searchResult = new DataverseSearchOut(-1, new[] { searchItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(searchResult);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn(searchText);
        var actualResult = await func.InvokeAsync(input, CancellationToken.None);

        Assert.True(actualResult.IsSuccess);

        var actual = actualResult.SuccessOrThrow().Customers;
        var expected = new CustomerItemSearchOut[]
        {
            new(accountId, title)
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task InvokeAsync_DataverseSearchResultIsSuccessWithoutName_ExpectSuccess()
    {
        var searchText = "Some Search Text";
        var accountId = Guid.Parse("1b91d06f-208d-4c1c-b630-0ee9996a8a59");

        var searchItem = new DataverseSearchItem(
            searchScore: -917.095,
            objectId: accountId,
            entityName: "Some entity name",
            extensionData: default);

        var searchResult = new DataverseSearchOut(-1, new[] { searchItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(searchResult);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn(searchText, 3);
        var actualResult = await func.InvokeAsync(input, CancellationToken.None);

        Assert.True(actualResult.IsSuccess);

        var actual = actualResult.SuccessOrThrow().Customers;
        var expected = new CustomerItemSearchOut[]
        {
            new(accountId, string.Empty)
        };

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsEmptyArray_ExpectEmptySuccess()
    {
        var dataverseOut = new DataverseSearchOut(71, default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn("Some Search text", 5);
        var actualResult = await func.InvokeAsync(input, default);

        Assert.True(actualResult.IsSuccess);
        Assert.Empty(actualResult.SuccessOrThrow().Customers);
    }
}