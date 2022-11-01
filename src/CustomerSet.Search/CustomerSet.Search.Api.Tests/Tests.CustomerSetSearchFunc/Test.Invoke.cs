using GGroupp.Infra;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
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
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var input = new CustomerSetSearchIn(searchString);
        var token = new CancellationToken(canceled: false);

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(input, token);

        var expected = new DataverseSearchIn($"*{searchString}*")
        {
            Entities = new[] { "account" }
        };

        mockDataverseApiClient.Verify(c => c.SearchAsync(expected, token), Times.Once);
    }

    [Theory]
    [InlineData(DataverseFailureCode.Throttling, CustomerSetSearchFailureCode.TooManyRequests)]
    [InlineData(DataverseFailureCode.UserNotEnabled, CustomerSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, CustomerSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, CustomerSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, CustomerSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unauthorized, CustomerSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unknown, CustomerSetSearchFailureCode.Unknown)]
    public async Task InvokeAsync_DataverseSearchResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, CustomerSetSearchFailureCode expectedFailureCode)
    {
        const string failureMessge = "Some failure message";
        var dataverseFailure = Failure.Create(sourceFailureCode, failureMessge);

        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseFailure);
        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn("Some search text", 5);
        var actual = await func.InvokeAsync(input, CancellationToken.None);

        var expected = Failure.Create(expectedFailureCode, failureMessge);
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
            }.ToFlatArray());

        var searchResult = new DataverseSearchOut(-1, new[] { searchItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(searchResult);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn(searchText);
        var actual = await func.InvokeAsync(input, CancellationToken.None);

        var expected = new CustomerSetSearchOut(
            customers: new CustomerItemSearchOut[]
            {
                new(accountId, title)
            });

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
        var actual = await func.InvokeAsync(input, CancellationToken.None);

        var expected = new CustomerSetSearchOut(
            customers: new CustomerItemSearchOut[]
            {
                new(accountId, string.Empty)
            });

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsEmptyArray_ExpectEmptySuccess()
    {
        var dataverseOut = new DataverseSearchOut(71, default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn("Some Search text", 5);

        var actual = await func.InvokeAsync(input, default);
        var expected = new CustomerSetSearchOut(default);

        Assert.Equal(expected, actual);
    }
}