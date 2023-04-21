using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GGroupp.Infra;
using Moq;
using Xunit;

namespace GGroupp.Internal.Support.Api.Test;

partial class SupportApiTest
{
    [Fact]
    public static async Task SearchCustomerSetAsync_InputIsNull_ExpectArgumentNullException()
    {
        var dataverseOut = new DataverseSearchOut(1, SomeDataverseCustomerItem.AsFlatArray());
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var api = new SupportApi(mockDataverseApiClient.Object);
        var cancellationToken = new CancellationToken(canceled: false);

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(TestAsync);
        Assert.Equal("input", ex.ParamName);

        async Task TestAsync()
            =>
            _ = await api.SearchCustomerSetAsync(null!, cancellationToken);
    }

    [Fact]
    public static void SearchCustomerSetAsync_CancellationTokenIsCanceled_ExpectTaskIsCanceled()
    {
        var dataverseOut = new DataverseSearchOut(51, SomeDataverseCustomerItem.AsFlatArray());
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var api = new SupportApi(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn("Some search text");
        var token = new CancellationToken(canceled: true);

        var actualTask = api.SearchCustomerSetAsync(input, token);
        Assert.True(actualTask.IsCanceled);
    }

    [Theory]
    [InlineData("\u043B\u044C\u0441")]
    [InlineData(Strings.Empty)]
    [InlineData(null)]
    public static async Task SearchCustomerSetAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce(
        string? searchString)
    {
        var dataverseOut = new DataverseSearchOut(17, SomeDataverseCustomerItem.AsFlatArray());
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var input = new CustomerSetSearchIn(searchString);
        var token = new CancellationToken(canceled: false);

        var api = new SupportApi(mockDataverseApiClient.Object);
        _ = await api.SearchCustomerSetAsync(input, token);

        var expected = new DataverseSearchIn($"*{searchString}*")
        {
            Entities = new("account")
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
    [InlineData(DataverseFailureCode.DuplicateRecord, CustomerSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unknown, CustomerSetSearchFailureCode.Unknown)]
    public static async Task SearchCustomerSetAsync_DataverseSearchResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, CustomerSetSearchFailureCode expectedFailureCode)
    {
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some failure message");
        var dataverseResult = Result.Failure(dataverseFailure).With<DataverseSearchOut>();

        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseResult);
        var api = new SupportApi(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn("Some search text")
        {
            Top = 5
        };

        var actual = await api.SearchCustomerSetAsync(input, CancellationToken.None);

        var expected = Failure.Create(expectedFailureCode, "Some failure message");
        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task SearchCustomerSetAsync_DataverseSearchResultIsSuccessWithName_ExpectSuccess()
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

        var api = new SupportApi(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn(searchText);
        var actual = await api.SearchCustomerSetAsync(input, CancellationToken.None);

        var expected = new CustomerSetSearchOut
        {
            Customers = new CustomerItemSearchOut(accountId, title).AsFlatArray()
        };

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task SearchCustomerSetAsync_DataverseSearchResultIsSuccessWithoutName_ExpectSuccess()
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

        var api = new SupportApi(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn(searchText)
        {
            Top = 3
        };

        var actual = await api.SearchCustomerSetAsync(input, CancellationToken.None);

        var expected = new CustomerSetSearchOut
        {
            Customers = new CustomerItemSearchOut(accountId, string.Empty).AsFlatArray()
        };

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task SearchCustomerSetAsync_SuccessResultIsEmptyArray_ExpectEmptySuccess()
    {
        var dataverseOut = new DataverseSearchOut(71, default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var api = new SupportApi(mockDataverseApiClient.Object);

        var input = new CustomerSetSearchIn("Some Search text")
        {
            Top = 5
        };

        var actual = await api.SearchCustomerSetAsync(input, default);
        var expected = default(CustomerSetSearchOut);

        Assert.StrictEqual(expected, actual);
    }
}