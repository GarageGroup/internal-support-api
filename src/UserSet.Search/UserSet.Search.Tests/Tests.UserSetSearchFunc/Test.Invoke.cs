using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DeepEqual.Syntax;
using GGroupp.Infra;
using Moq;
using Xunit;

namespace GGroupp.Internal.Support.UserSet.Search.Tests;

partial class UserSetSearchFuncTest
{
    [Fact]
    public void InvokeAsync_CancellationTokenIsCanceled_ExpectTaskIsCanceled()
    {
        var dataverseOut = new DataverseSearchOut(51, new[] { SomeDataverseSearchItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new UserSetSearchIn("Some text", null);
        var token = new CancellationToken(canceled: true);

        var actualTask = func.InvokeAsync(input, token);
        Assert.True(actualTask.IsCanceled);
    }

    [Theory]
    [InlineData(null, 5)]
    [InlineData("", null)]
    [InlineData("\u043B\u044C\u0441", 3)]
    [InlineData("Some text", null)]
    public async Task InvokeAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce(string? searchString, int? top)
    {
        var dataverseOut = new DataverseSearchOut(17, new[] { SomeDataverseSearchItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut, IsMatchDataverseInput);

        var input = new UserSetSearchIn(searchString, top);
        var token = new CancellationToken(canceled: false);

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(input, token);

        mockDataverseApiClient.Verify(c => c.SearchAsync(It.IsAny<DataverseSearchIn>(), token), Times.Once);

        void IsMatchDataverseInput(DataverseSearchIn actual)
        {
            var expected = new DataverseSearchIn($"*{searchString}*")
            {
                Entities = new[] { "systemuser" },
                Top = top
            };

            actual.ShouldDeepEqual(expected);
        }
    }

    [Theory]
    [InlineData(DataverseFailureCode.Throttling, UserSetSearchFailureCode.TooManyRequests)]
    [InlineData(DataverseFailureCode.UserNotEnabled, UserSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, UserSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, UserSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, UserSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unknown, UserSetSearchFailureCode.Unknown)]
    public async Task InvokeAsync_DataverseSearchResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, UserSetSearchFailureCode expectedFailureCode)
    {
        const string failureMessge = "Some failure message";
        var dataverseFailure = Failure.Create(sourceFailureCode, failureMessge);

        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseFailure);
        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new UserSetSearchIn("Some search text", 5);
        var actual = await func.InvokeAsync(input, CancellationToken.None);

        var expected = Failure.Create(expectedFailureCode, failureMessge);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task InvokeAsync_DataverseSearchResultIsSuccess_ExpectSuccess()
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
            });

        var secondFullName = "Second Name";

        var secondItem = new DataverseSearchItem(
            searchScore: 155.7,
            objectId: Guid.Parse("92fb1ea1-ecfd-40e2-8034-1ff38b1b2fe8"),
            entityName: "Second EntityName",
            extensionData: new Dictionary<string, DataverseSearchJsonValue>
            {
                ["fullname"] = new(JsonSerializer.SerializeToElement(secondFullName))
            });

        var thirdItem = new DataverseSearchItem(
            searchScore: -200,
            objectId: Guid.Parse("92fb1ea1-ecfd-40e2-8034-1ff38b1b2fe8"),
            entityName: "Third Entity Name",
            extensionData: default);

        var searchResult = new DataverseSearchOut(-1, new[] { firstItem, secondItem, thirdItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(searchResult);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new UserSetSearchIn("Some text", 7);
        var actualResult = await func.InvokeAsync(input, CancellationToken.None);

        Assert.True(actualResult.IsSuccess);

        var actual = actualResult.SuccessOrThrow().Users;
        var expected = new UserItemSearchOut[]
        {
            new(firstItem.ObjectId, firstFullName),
            new(secondItem.ObjectId, secondFullName),
            new(thirdItem.ObjectId, string.Empty)
        };

        Assert.Equal(expected, actual);
    }
}