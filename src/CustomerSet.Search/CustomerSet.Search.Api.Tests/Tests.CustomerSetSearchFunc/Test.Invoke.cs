using DeepEqual.Syntax;
using GGroupp.Infra;
using Moq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GGroupp.Internal.Support.CustomerSet.Search.Api.Tests;

partial class CustomerSetSearchFuncTest
{
    [Fact]
    public async Task InvokeAsync_InputIsNull_ExpectArgumentNullExcepcion()
    {
        var success = new DataverseSearchOut(0, null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => func.InvokeAsync(null!, CancellationToken.None).AsTask());
        Assert.Equal("input", exception.ParamName);
    }

    [Fact]
    public void InvokeAsync_CancellationTokenIsCanceled_ExpectTaskIsCanceled()
    {
        var success = new DataverseSearchOut(0, null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var valueTask = func.InvokeAsync(new(string.Empty), new CancellationToken(true));
        Assert.True(valueTask.IsCanceled);
    }

    [Theory]
    [InlineData("\u043B\u044C\u0441")]
    [InlineData(Strings.Empty)]
    [InlineData(null)]
    public async Task InvokeAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce(
        string searchString)
    {
        var success = new DataverseSearchOut(0, null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success, IsMatchDataverseInput);

        var token = new CancellationToken(false);

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(new(searchString), token);

        mockDataverseApiClient.Verify(
            c => c.SearchAsync(
                It.IsAny<DataverseSearchIn>(), token),
            Times.Once);

        void IsMatchDataverseInput(DataverseSearchIn actual)
        {
            var expected = new DataverseSearchIn(
                $"*{searchString}*")
            { Entities = new ReadOnlyCollection<string>(new[] { "account" }) };
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

        var jsElementNullable = JsonSerializer.Deserialize<JsonExt>($"{{\"name\":\"{title}\"}}")?.ExtensionData?.First().Value;
        var jsElement = 
            jsElementNullable.HasValue ? 
            jsElementNullable.Value : 
            throw new ArgumentNullException(nameof(jsElementNullable));

        var success = new DataverseSearchOut(-1, new ReadOnlyCollection<DataverseSearchItem>(new DataverseSearchItem[] 
        {
            new(
                searchScore:2,
                entityName:"accountId",
                objectId:accountId,
                extensionData: new(new Dictionary<string, DataverseSearchJsonValue>()
                { 
                    {"name", new DataverseSearchJsonValue(jsElement) }
                }))
        }));
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
        var success = new DataverseSearchOut(0, new DataverseSearchItem[0]);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(new(string.Empty), default);

        Assert.True(actualResult.IsSuccess);
        var customers = actualResult.SuccessOrThrow().Customers;

        Assert.Empty(customers);
    }
}
