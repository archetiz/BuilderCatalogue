using AutoFixture.Xunit2;
using BrickApi.Client;
using BrickApi.Client.Api.User.ById.Item;
using BrickApi.Client.Api.User.ByUsername.Item;
using BrickApi.Client.Api.Users;
using BuilderCatalogue.DTOs;
using BuilderCatalogue.Managers;
using FluentAssertions;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Serialization;
using Moq;

#pragma warning disable CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
                               // Reason: The adapter mock needs a generic non-nullable parameter, so there is a warning if we pass null as a mock result, but the actual returned result is nullable, so it is fine to pass null

namespace BuilderCatalogue.Tests.Managers
{
    public class UserDataManagerTests
    {
        private readonly Mock<IRequestAdapter> _requestAdapterMock = new();
        private readonly BrickApiClient _apiClient;

        public UserDataManagerTests()
        {
            _apiClient = new BrickApiClient(_requestAdapterMock.Object);
        }

        private UserDataManager CreateManager() => new(_apiClient);

        [Theory, AutoData]
        public async Task GetAllUsers_ReturnsFullListNormally(UsersGetResponse users)
        {
            // Arrange
            MockGETRequestResult(users);

            var sut = CreateManager();

            // Act
            var result = await sut.GetAllUsers();

            // Assert
            result.Should().Equal(users.Users);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsEmptyListWhenResultIsNullOrNotFound()
        {
            // Arrange
            MockGETRequestResult<UsersGetResponse?>(null);

            var sut = CreateManager();

            // Act
            var result = await sut.GetAllUsers();

            // Assert
            result.Should().BeEmpty();
        }

        [Theory, AutoData]
        public async Task GetUserDataByName_ShouldReturnDetailedUserDataIfFound(WithUsernameGetResponse userDataRequestByNameResult, UserData userData)
        {
            // Arrange
            userDataRequestByNameResult.Id = userData.Id;
            userDataRequestByNameResult.Username = userData.Name;

            MockGETRequestResult(userDataRequestByNameResult);

            var sut = new Mock<UserDataManager>(_apiClient);

            sut.Setup(udm => udm.GetUserDataById(userDataRequestByNameResult.Id)).ReturnsAsync(userData);

            // Act
            var result = await sut.Object.GetUserDataByName(userData.Name);

            // Assert
            result.Should().BeEquivalentTo(userData);
        }

        [Theory, AutoData]
        public async Task GetUserDataByName_ShouldReturnNullIfUserIsNotFound(string username)
        {
            // Arrange
            MockGETRequestResult<WithUsernameGetResponse?>(null);

            var sut = CreateManager();

            // Act
            var result = await sut.GetUserDataByName(username);

            // Assert
            result.Should().BeNull();
        }

        [Theory, AutoData]
        public async Task GetUserDataById_ShouldReturnDetailedUserDataIfFound(ByIdGetResponse userDataRequestByIdResult)
        {
            // Arrange
            userDataRequestByIdResult.Collection =
                [
                    new() {
                       PieceId = "100",
                       Variants = [
                            new() {
                                Color = "1",
                                Count = 2
                            },
                            new() {
                                Color = "2",
                                Count = 3
                            }
                        ]
                    },
                    new() {
                       PieceId = "300",
                       Variants = [
                            new() {
                                Color = "100",
                                Count = 100
                            }
                        ]
                    }
                ];

            MockGETRequestResult(userDataRequestByIdResult);

            var sut = CreateManager();

            // Act
            var result = await sut.GetUserDataById(userDataRequestByIdResult.Id!);

            // Assert
            var expectedResult = new UserData(userDataRequestByIdResult.Id!, userDataRequestByIdResult.Username!, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Theory, AutoData]
        public async Task GetUserDataById_ShouldReturnNullIfUserIsNotFound(Guid userId)
        {
            // Arrange
            MockGETRequestResult<ByIdGetResponse?>(null);

            var sut = CreateManager();

            // Act
            var result = await sut.GetUserDataById(userId.ToString());

            // Assert
            result.Should().BeNull();
        }

        private void MockGETRequestResult<TResponse>(TResponse mockResponse) where TResponse : IParsable
            => _requestAdapterMock.Setup(ra => ra.SendAsync(It.IsAny<RequestInformation>(),
                                             It.IsAny<ParsableFactory<TResponse>>(),
                                             It.IsAny<Dictionary<string, ParsableFactory<IParsable>>>(),
                                             It.IsAny<CancellationToken>()))
                                            .ReturnsAsync(mockResponse);
    }
}

#pragma warning restore CS8631 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match constraint type.
