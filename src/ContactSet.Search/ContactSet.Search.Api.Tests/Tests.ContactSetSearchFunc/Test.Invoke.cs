using DeepEqual.Syntax;
using GGroupp.Infra;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GGroupp.Internal.Support.ContactSet.Search.Api.Tests;

partial class ContactSetSearchFuncTest
{

    [Fact]
    public void InvokeAsync_CancellationTokenIsCanceled_ExpectTaskIsCanceled()
    {
        var success = new DataverseSearchOut(0, null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var valueTask = func.InvokeAsync(new(string.Empty, new()), new CancellationToken(true));
        Assert.True(valueTask.IsCanceled);
    }

    [Theory]
    [InlineData("\u043B\u044C\u0441")]
    [InlineData(Strings.Empty)]
    [InlineData(null)]
    public async Task InvokeAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce(
        string searchString)
    {
        const string guidString = "2a5d892f-1400-ec11-94ef-000d3a4a099f";
        var success = new DataverseSearchOut(0, null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success, IsMatchDataverseInput);

        var token = new CancellationToken(false);

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(new(searchString, new(guidString)), token);

        mockDataverseApiClient.Verify(
            c => c.SearchAsync(
                It.IsAny<DataverseSearchIn>(), token),
            Times.Once);

        void IsMatchDataverseInput(DataverseSearchIn actual)
        {
            var expected = new DataverseSearchIn(
                $"*{searchString}*")
            { 
                Entities = new ReadOnlyCollection<string>(new[] { "contact" }), 
                Filter = $"parentcustomerid eq '{Guid.Parse(guidString).ToString("D", CultureInfo.InvariantCulture)}'"
            };
            actual.ShouldDeepEqual(expected);
        }
    }

    [Fact]
    public async Task InvokeAsync_CancellationTokenIsNotCanceledDefaultInput_ExpectCallDataVerseApiClientOnce()
    {
        var success = new DataverseSearchOut(0, null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success, IsMatchDataverseInput);

        var token = new CancellationToken(false);

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(default, token);

        mockDataverseApiClient.Verify(
            c => c.SearchAsync(
                It.IsAny<DataverseSearchIn>(), token),
            Times.Once);

        void IsMatchDataverseInput(DataverseSearchIn actual)
        {
            var expected = new DataverseSearchIn(
                $"**")
            {
                Entities = new ReadOnlyCollection<string>(new[] { "contact" }),
                Filter = $"parentcustomerid eq '{new Guid().ToString("D", CultureInfo.InvariantCulture)}'"
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
    public async Task InvokeAsync_FailureResultIsGiven_ExpectFailure(int failureCode)
    {
        const string failureMessge = "Bad request";
        var failure = Failure.Create(failureCode, failureMessge);
        var mockDataverseApiClient = CreateMockDataverseApiClient(failure);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(new(string.Empty, new()), CancellationToken.None);

        var expectedFailure = Failure.Create(ContactSetSearchFailureCode.Unknown, failureMessge);
        Assert.Equal(expectedFailure, actualResult);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsGiven_ExpectSuccessResult()
    {
        var searchText = "вик";
        var contactId = Guid.Parse("1b91d06f-208d-4c1c-b630-0ee9996a8a59");
        var fullName = "Виктор Васнецов";

        var jsElementNullable = JsonSerializer.Deserialize<JsonExt>($"{{\"fullname\":\"{fullName}\"}}")?.ExtensionData?.First().Value;
        var jsElement = 
            jsElementNullable.HasValue ? 
            jsElementNullable.Value : 
            throw new ArgumentNullException(nameof(jsElementNullable));

        var success = new DataverseSearchOut(-1, new ReadOnlyCollection<DataverseSearchItem>(new DataverseSearchItem[] 
        {
            new(
                searchScore:2,
                entityName:"accountId",
                objectId:contactId,
                extensionData: new(new Dictionary<string, DataverseSearchJsonValue>()
                { 
                    {"fullname", new DataverseSearchJsonValue(jsElement) }
                }))
        }));
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = (await func.InvokeAsync(new(searchText, new()), default));
        Assert.True(actualResult.IsSuccess);

        var actual = actualResult.SuccessOrThrow().Contacts;
        var expected = new ContactItemSearchOut[] { new(contactId, fullName) };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsEmptyArray_ExpectSuccessResult()
    {
        var success = new DataverseSearchOut(0, new DataverseSearchItem[0]);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(default, default);

        Assert.True(actualResult.IsSuccess);
        var customers = actualResult.SuccessOrThrow().Contacts;

        Assert.Empty(customers);
    }
}
