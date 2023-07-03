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
    public static async Task SearchContactSetAsync_InputIsNull_ExpectArgumentNullException()
    {
        var dataverseOut = new DataverseSearchOut(1, SomeDataverseContactItem.AsFlatArray());
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var api = new SupportApi(mockDataverseApiClient.Object);
        var cancellationToken = new CancellationToken(canceled: false);

        var ex = await Assert.ThrowsAsync<ArgumentNullException>(TestAsync);
        Assert.Equal("input", ex.ParamName);

        async Task TestAsync()
            =>
            _ = await api.SearchContactSetAsync(null!, cancellationToken);
    }

    [Fact]
    public static void SearchContactSetAsync_CancellationTokenIsCanceled_ExpectTaskIsCanceled()
    {
        var dataverseOut = new DataverseSearchOut(1, SomeDataverseContactItem.AsFlatArray());
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var api = new SupportApi(mockDataverseApiClient.Object);
        var cancellationToken = new CancellationToken(canceled: true);

        var valueTask = api.SearchContactSetAsync(SomeContactSetSearchInput, cancellationToken);
        Assert.True(valueTask.IsCanceled);
    }

    [Theory]
    [InlineData("\u043B\u044C\u0441", null)]
    [InlineData(Strings.Empty, 15)]
    [InlineData(null, -1)]
    public static async Task SearchContactSetAsync_CancellationTokenIsNotCanceledAndTop_ExpectCallDataVerseApiClientOnce(
        string searchString, int? top)
    {
        var dataverseOut = new DataverseSearchOut(15, SomeDataverseContactItem.AsFlatArray());
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var input = new ContactSetSearchIn(
            searchText: searchString,
            customerId: Guid.Parse("2a5d892f-1400-ec11-94ef-000d3a4a099f"))
        {
            Top = top
        };

        var cancellationToken = new CancellationToken(canceled: false);

        var api = new SupportApi(mockDataverseApiClient.Object);
        _ = await api.SearchContactSetAsync(input, cancellationToken);

        var expected = new DataverseSearchIn($"*{searchString}*")
        {
            Entities = new("contact"),
            Filter = "parentcustomerid eq '2a5d892f-1400-ec11-94ef-000d3a4a099f'",
            Top = top
        };

        mockDataverseApiClient.Verify(c => c.SearchAsync(expected, cancellationToken), Times.Once);
    }

    [Theory]
    [InlineData(DataverseFailureCode.Unknown, ContactSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Unauthorized, ContactSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.PicklistValueOutOfRange, ContactSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.PrivilegeDenied, ContactSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.RecordNotFound, ContactSetSearchFailureCode.Unknown)]
    [InlineData(DataverseFailureCode.Throttling, ContactSetSearchFailureCode.TooManyRequests)]
    [InlineData(DataverseFailureCode.UserNotEnabled, ContactSetSearchFailureCode.NotAllowed)]
    [InlineData(DataverseFailureCode.SearchableEntityNotFound, ContactSetSearchFailureCode.NotAllowed)]
    public static async Task SearchContactSetAsync_DataverseResultIsFailure_ExpectFailure(
        DataverseFailureCode sourceFailureCode, ContactSetSearchFailureCode expectedFailureCode)
    {
        var dataverseFailure = Failure.Create(sourceFailureCode, "Some Failure message");
        var dataverseResult = Result.Failure(dataverseFailure).With<DataverseSearchOut>();

        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseResult);
        var api = new SupportApi(mockDataverseApiClient.Object);

        var actual = await api.SearchContactSetAsync(SomeContactSetSearchInput, CancellationToken.None);
        var expected = Failure.Create(expectedFailureCode, dataverseFailure.FailureMessage);

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task SearchContactSetAsync_DataverseResultIsSuccessNotEmpty_ExpectSuccessNotEmpty()
    {
        var firstContactId = Guid.Parse("604fae90-7894-48ea-92bf-e888bf0ce6ca");

        var firstDataverseSearchItem = new DataverseSearchItem(
            searchScore: -81263.91,
            objectId: firstContactId,
            entityName: "First entity name",
            extensionData: default);

        var secondContactId = Guid.Parse("eaf4a5e1-3303-4ec1-84cd-626b3828b13b");
        var secondFullName = "Some Full Name";

        var secondDataverseSearchItem = new DataverseSearchItem(
            searchScore: 1000,
            objectId: secondContactId,
            entityName: "SecondEntityName",
            extensionData: new Dictionary<string, DataverseSearchJsonValue>
            {
                ["fullName"] = new(JsonSerializer.SerializeToElement("Some value")),
                ["fullname"] = new(JsonSerializer.SerializeToElement(secondFullName))
            }.ToFlatArray());

        var dataverseOut = new DataverseSearchOut(
            totalRecordCount: 0, 
            value: new(firstDataverseSearchItem, secondDataverseSearchItem));

        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);
        var api = new SupportApi(mockDataverseApiClient.Object);

        var actual = await api.SearchContactSetAsync(SomeContactSetSearchInput, default);

        var expected = new ContactSetSearchOut
        {
            Contacts = new ContactItemSearchOut[]
            {
                new(firstContactId, string.Empty),
                new(secondContactId, secondFullName)
            }
        };

        Assert.StrictEqual(expected, actual);
    }

    [Fact]
    public static async Task SearchContactSetAsync_DataverseResultIsSuccessEmpty_ExpectSuccessEmpty()
    {
        var dataverseOut = new DataverseSearchOut(5, default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var api = new SupportApi(mockDataverseApiClient.Object);

        var actual = await api.SearchContactSetAsync(SomeContactSetSearchInput, CancellationToken.None);
        var expected = default(ContactSetSearchOut);

        Assert.StrictEqual(expected, actual);
    }
}