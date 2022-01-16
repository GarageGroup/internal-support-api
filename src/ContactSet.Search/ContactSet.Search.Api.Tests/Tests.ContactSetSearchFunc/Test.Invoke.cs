using DeepEqual.Syntax;
using GGroupp.Infra;
using Moq;
using System;
using System.Collections.Generic;
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
        var dataverseOut = new DataverseSearchOut(1, new[] { SomeDataverseSearchItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = SomeInput;
        var token = new CancellationToken(canceled: true);

        var valueTask = func.InvokeAsync(input, token);
        Assert.True(valueTask.IsCanceled);
    }

    [Theory]
    [InlineData("\u043B\u044C\u0441", null)]
    [InlineData(Strings.Empty, 15)]
    [InlineData(null, -1)]
    public async Task InvokeAsync_CancellationTokenIsNotCanceledAndTop_ExpectCallDataVerseApiClientOnce(
        string searchString, int? top)
    {
        var dataverseOut = new DataverseSearchOut(15, new[] { SomeDataverseSearchItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut, IsMatchDataverseInput);

        const string customerId = "2a5d892f-1400-ec11-94ef-000d3a4a099f";
        var input = new ContactSetSearchIn(
            searchText: searchString,
            customerId: Guid.Parse(customerId),
            top: top);

        var token = new CancellationToken(canceled: false);

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(input, token);

        mockDataverseApiClient.Verify(c => c.SearchAsync(It.IsAny<DataverseSearchIn>(), token), Times.Once);

        void IsMatchDataverseInput(DataverseSearchIn actual)
        {
            var expected = new DataverseSearchIn($"*{searchString}*")
            { 
                Entities = new[] { "contact" }, 
                Filter = $"parentcustomerid eq '{customerId}'",
                Top = top
            };
            actual.ShouldDeepEqual(expected);
        }
    }

    [Fact]
    public async Task InvokeAsync_CancellationTokenIsNotCanceledAndInputIsDefault_ExpectCallDataVerseApiClientOnce()
    {
        var dataverseOut = new DataverseSearchOut(1, new[] { SomeDataverseSearchItem });
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut, IsMatchDataverseInput);

        var token = new CancellationToken(false);

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(default, token);

        mockDataverseApiClient.Verify(c => c.SearchAsync(It.IsAny<DataverseSearchIn>(), token), Times.Once);

        static void IsMatchDataverseInput(DataverseSearchIn actual)
        {
            var expected = new DataverseSearchIn($"**")
            {
                Entities = new[] { "contact" },
                Filter = $"parentcustomerid eq '00000000-0000-0000-0000-000000000000'"
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
    public async Task InvokeAsync_DataverseResultIsFailure_ExpectFailure(int failureCode)
    {
        var dataverseFailure = Failure.Create(failureCode, "Some Failure message");
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseFailure);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actual = await func.InvokeAsync(SomeInput, CancellationToken.None);

        var expected = Failure.Create(ContactSetSearchFailureCode.Unknown, dataverseFailure.FailureMessage);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task InvokeAsync_DataverseResultIsSuccessNotEmpty_ExpectSuccessNotEmpty()
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
            });

        var dataverseOut = new DataverseSearchOut(
            totalRecordCount: 0, 
            value: new[] { firstDataverseSearchItem, secondDataverseSearchItem });

        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);
        var func = CreateFunc(mockDataverseApiClient.Object);

        var actualResult = await func.InvokeAsync(SomeInput, default);

        Assert.True(actualResult.IsSuccess);
        var actual = actualResult.SuccessOrThrow().Contacts;

        var expected = new ContactItemSearchOut[]
        {
            new(firstContactId, string.Empty),
            new(secondContactId, secondFullName)
        };
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task InvokeAsync_DataverseResultIsSuccessEmpty_ExpectSuccessEmpty()
    {
        var dataverseOut = new DataverseSearchOut(5, Array.Empty<DataverseSearchItem>());
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(SomeInput, CancellationToken.None);

        Assert.True(actualResult.IsSuccess);
        Assert.Empty(actualResult.SuccessOrThrow().Contacts);
    }
}