using DeepEqual.Syntax;
using GGroupp.Infra;
using Moq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace GGroupp.Internal.Support.User.Get.Api.Tests;

partial class UserGetFuncTest
{
    [Fact]
    public void InvokeAsync_CancellationTokenIsCanceled_ExpectValueTaskIsCanceled()
    {
        var dataverseOut = new DataverseEntityGetOut<UserJsonGetOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new UserGetIn(SomeActiveDirectoryGuid);
        var token = new CancellationToken(canceled: true);

        var actual = func.InvokeAsync(input, token);
        Assert.True(actual.IsCanceled);
    }

    [Fact]
    public async  Task InvokeAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce()
    {
        var dataverseOut = new DataverseEntityGetOut<UserJsonGetOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut, IsMatchDataverseInput);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new UserGetIn(SomeActiveDirectoryGuid);
        var token = new CancellationToken(false);

        _ = await func.InvokeAsync(input, token);

        mockDataverseApiClient.Verify(
            c => c.GetEntityAsync<UserJsonGetOut>(It.IsAny<DataverseEntityGetIn>(), token),
            Times.Once);

        static void IsMatchDataverseInput(DataverseEntityGetIn actual)
        {
            var expected = new DataverseEntityGetIn(
                entityPluralName: "systemusers",
                entityKey: new DataverseAlternateKey(
                    new KeyValuePair<string, string>[]
                    {
                        new("azureactivedirectoryobjectid", SomeActiveDirectoryGuid.ToString("D", CultureInfo.InvariantCulture))
                    }),
                selectFields: new[] { "systemuserid" });

            actual.ShouldDeepEqual(expected);
        }
    }

    [Theory]
    [InlineData(404, UserGetFailureCode.Unknown)]
    [InlineData(int.MinValue, UserGetFailureCode.Unknown)]
    [InlineData(int.MaxValue, UserGetFailureCode.Unknown)]
    [InlineData(0, UserGetFailureCode.Unknown)]
    [InlineData(DataverseNotFoundStatusCode, UserGetFailureCode.NotFound)]
    public async Task InvokeAsync_DataverseResultIsFailure_ExpectFailure(int failureCode, UserGetFailureCode userGetFailureCode)
    {
        var dataverseFailure = Failure.Create(failureCode, "Some failure message");
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseFailure);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new UserGetIn(SomeActiveDirectoryGuid);
        var actual = await func.InvokeAsync(input, CancellationToken.None);

        var expected = Failure.Create(userGetFailureCode, dataverseFailure.FailureMessage);
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task InvokeAsync_DataverseResultIsSuccess_ExpectSuccess()
    {
        var dataverseUser = new UserJsonGetOut
        {
            SystemUserId = SomeSystemUserId
        };

        var dataverseOut = new DataverseEntityGetOut<UserJsonGetOut>(dataverseUser);
        var mockDataverseApiClient = CreateMockDataverseApiClient(dataverseOut);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new UserGetIn(SomeActiveDirectoryGuid);
        var actual = await func.InvokeAsync(input, CancellationToken.None);

        var expected = new UserGetOut(dataverseUser.SystemUserId);
        Assert.Equal(expected, actual);
    }
}
