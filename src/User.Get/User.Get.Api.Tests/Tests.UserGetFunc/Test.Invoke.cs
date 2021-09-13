using DeepEqual.Syntax;
using GGroupp.Infra;
using Moq;
using System.Collections.ObjectModel;
using System.Globalization;
using Xunit;

namespace GGroupp.Internal.Support.User.Get.Api.Tests;

partial class UserGetFuncTest
{
    [Fact]
    public async Task InvokeAsync_InputIsNull_ExpectArgumentNullExcepcion()
    {
        var success = new DataverseEntityGetOut<UserGetJsonOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var exception = await Assert.ThrowsAsync<ArgumentNullException>(() => func.InvokeAsync(null!, CancellationToken.None).AsTask());
        Assert.Equal("input", exception.ParamName);
    }

    [Fact]
    public void InvokeAsync_CancellationTokenIsCanceled_ExpectValueTaskIsCanceled()
    {
        var success = new DataverseEntityGetOut<UserGetJsonOut>(default);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);

        var input = new UserGetIn(ValidActiveDirectoryGuid);
        var token = new CancellationToken(canceled: true);

        var actual = func.InvokeAsync(input, token);
        Assert.True(actual.IsCanceled);
    }

    [Fact]
    public async  Task InvokeAsync_CancellationTokenIsNotCanceled_ExpectCallDataVerseApiClientOnce()
    {
        var success = new DataverseEntityGetOut<UserGetJsonOut>(null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success, IsMatchDataverseInput);

        var token = new CancellationToken(false);

        var func = CreateFunc(mockDataverseApiClient.Object);
        _ = await func.InvokeAsync(new(ValidActiveDirectoryGuid), token);

        mockDataverseApiClient.Verify(
            c => c.GetEntityAsync<UserGetJsonOut>(
                It.IsAny<DataverseEntityGetIn>(), token), 
            Times.Once);

        static void IsMatchDataverseInput(DataverseEntityGetIn actual)
        {
            var expected = new DataverseEntityGetIn(
                    entityPluralName: "systemusers",
                    entityKey: new DataverseAlternateKey(
                                new KeyValuePair<string, string>[]
                                {
                                    new("azureactivedirectoryobjectid", ValidActiveDirectoryGuid.ToString("D", CultureInfo.InvariantCulture))
                                }),
                    selectFields: new ReadOnlyCollection<string>(new[] { "systemuserid" }));

            actual.ShouldDeepEqual(expected);
        }
    }

    [Theory]
    [InlineData(404, UserGetFailureCode.Unknown)]
    [InlineData(int.MinValue, UserGetFailureCode.Unknown)]
    [InlineData(int.MaxValue, UserGetFailureCode.Unknown)]
    [InlineData(0, UserGetFailureCode.Unknown)]
    [InlineData(DataverseNotFoundStatusCode, UserGetFailureCode.NotFound)]
    public async Task InvokeAsync_FailureResultIsGiven_ExpectFailure(int failureCode, UserGetFailureCode userGetFailureCode)
    {
        const string failureMessge = "Bad request";
        var failure = Failure.Create(failureCode, failureMessge);
        var mockDataverseApiClient = CreateMockDataverseApiClient(failure);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(new(ValidActiveDirectoryGuid), CancellationToken.None);

        var expectedFailure = Failure.Create(userGetFailureCode, failureMessge);
        Assert.Equal(expectedFailure, actualResult);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsGiven_ExpectSuccessResult()
    {
        var success = new DataverseEntityGetOut<UserGetJsonOut>(new(ValidSystemUserId));
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = (await func.InvokeAsync(new(ValidActiveDirectoryGuid), default)).SuccessOrThrow();

        var expectedFailure = new UserGetOut(ValidSystemUserId);
        Assert.Equal(expectedFailure.SystemUserId, actualResult.SystemUserId);
    }

    [Fact]
    public async Task InvokeAsync_SuccessResultIsNull_ExpectSuccessResult()
    {
        var success = new DataverseEntityGetOut<UserGetJsonOut>(null);
        var mockDataverseApiClient = CreateMockDataverseApiClient(success);

        var func = CreateFunc(mockDataverseApiClient.Object);
        var actualResult = await func.InvokeAsync(new(ValidActiveDirectoryGuid), default);

        var expected = new UserGetOut(default);
        Assert.Equal(expected, actualResult.SuccessOrThrow());
    }
}
