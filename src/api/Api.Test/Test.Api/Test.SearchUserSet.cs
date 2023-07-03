using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GarageGroup.Infra;
using Moq;
using Xunit;

namespace GarageGroup.Internal.Support.Api.Test;

partial class SupportApiTest
{
    [Fact]
    public static async Task SearchUserSetAsync_InputIsNull_ExpectArgumentNullException()
    {
        var dataverseOut = new DataverseSearchOut(1, SomeDataverseCustomerItem.AsFlatArray());
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var api = new SupportApi(mockDataverseApiClient.Object);
        var cancellationToken = new CancellationToken(canceled: false);

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(TestAsync);
        Assert.Equal("input", ex.ParamName);

        async Task TestAsync()
            =>
            _ = await api.SearchUserSetAsync(null!, cancellationToken);
    }

    [Fact]
    public static void SearchUserSetAsync_CancellationTokenIsCanceled_ExpectTaskIsCanceled()
    {
        var dataverseOut = new DataverseSearchOut(51, SomeDataverseUserItem.AsFlatArray());
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var api = new SupportApi(mockDataverseApiClient.Object);

        var input = new UserSetSearchIn("Some text");
        var token = new CancellationToken(canceled: true);

        var actualTask = api.SearchUserSetAsync(input, token);
        Assert.True(actualTask.IsCanceled);
    }

    [Theory]
    [InlineData(null, 5)]
    [InlineData(Strings.Empty, null)]
    [InlineData("\u043B\u044C\u0441", 3)]
    [InlineData("Some text", -1)]
    public static async Task SearchUserSetAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce(
        string? searchString, int? top)
    {
        var dataverseOut = new DataverseSearchOut(17, SomeDataverseUserItem.AsFlatArray());
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var input = new UserSetSearchIn(searchString)
        {
            Top = top
        };

        var token = new CancellationToken(canceled: false);

        var api = new SupportApi(mockDataverseApiClient.Object);
        _ = await api.SearchUserSetAsync(input, token);

        var expected = new DataverseSearchIn($"*{searchString}*")
        {
            Entities = new("systemuser"),
            Top = top
        };

        mockDataverseApiClient.Verify(c => c.SearchAsync(expected, token), Times.Once);
    }

    [Theory]
    [InlineData(DataverseFailureCode.Throttling, UserSetSearchFailureCode.TooManyRequests)]
    [InlineData(DataverseFailureCode.UserNotEnabled, UserSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, UserSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, UserSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, UserSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unauthorized, UserSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.DuplicateRecord, UserSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unknown, UserSetSearchFailureCode.Unknown)]
    public static async Task SearchUserSetAsync_DataverseSearchResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, UserSetSearchFailureCode expectedFailureCode)
    {
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some error message");
        var dataverseResult = Result.Failure(dataverseFailure).With<DataverseSearchOut>();

        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseResult);
        var api = new SupportApi(mockDataverseApiClient.Object);

        var input = new UserSetSearchIn("Some search text")
        {
            Top = 5
        };

        var actual = await api.SearchUserSetAsync(input, CancellationToken.None);
        var expected = Failure.Create(expectedFailureCode, "Some error message");

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task SearchUserSetAsync_DataverseSearchResultIsSuccess_ExpectSuccess()
    {
        var firstFullName = string.Empty;

        var firstItem = new DataverseSearchItem(
            searchScore: 217.5,
            objectId: Guid.Parse("d18417f9-3e5d-4836-8d7d-41cd44baafc4"),
            entityName: "First Entity name",
            extensionData: new Dictionary<string, DataverseSearchJsonValue>
            {
                ["fullname"] = new(JsonSerializer.SerializeToElement(firstFullName)),
                ["fullName"] = new(JsonSerializer.SerializeToElement("Some Name")),
                ["name"] = new(JsonSerializer.SerializeToElement("Some value")),
                ["some"] = new(JsonSerializer.SerializeToElement(15))
            }.ToFlatArray());

        var secondFullName = "Second Name";

        var secondItem = new DataverseSearchItem(
            searchScore: 155.7,
            objectId: Guid.Parse("92fb1ea1-ecfd-40e2-8034-1ff38b1b2fe8"),
            entityName: "Second EntityName",
            extensionData: new Dictionary<string, DataverseSearchJsonValue>
            {
                ["fullname"] = new(JsonSerializer.SerializeToElement(secondFullName))
            }.ToFlatArray());

        var thirdItem = new DataverseSearchItem(
            searchScore: -200,
            objectId: Guid.Parse("92fb1ea1-ecfd-40e2-8034-1ff38b1b2fe8"),
            entityName: "Third Entity Name",
            extensionData: default);

        var searchResult = new DataverseSearchOut(-1, new[] { firstItem, secondItem, thirdItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(searchResult);

        var api = new SupportApi(mockDataverseApiClient.Object);

        var input = new UserSetSearchIn("Some text")
        {
            Top = 7
        };

        var actual = await api.SearchUserSetAsync(input, CancellationToken.None);

        var expected = new UserSetSearchOut
        {
            Users = new(
                new(firstItem.ObjectId, firstFullName),
                new(secondItem.ObjectId, secondFullName),
                new(thirdItem.ObjectId, string.Empty))
        };

        Assert.StrictEqual(expected, actual);
    }
}