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

        [Fact]
        public async Task GetAllUsers_ReturnsFullListNormally()
        {
            // Arrange
            var usersMock = new UsersGetResponse
            {
                Users =
                [
                    new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Username = "TestUser1",
                        BrickCount = 42,
                        Location = "Here"
                    },
                    new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Username = "TestUser2",
                        BrickCount = 43,
                        Location = "There"
                    },
                ]
            };

            MockGETRequestResult(usersMock);

            var sut = CreateManager();

            // Act
            var result = await sut.GetAllUsers();

            // Assert
            result.Should().Equal(usersMock.Users);
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

        [Fact]
        public async Task GetUserDataByName_ShouldReturnDetailedUserDataIfFound()
        {
            // Arrange
            var username = "TestUser";
            
            var userDataRequestByNameResult = new WithUsernameGetResponse
            {
                Id = Guid.NewGuid().ToString(),
                Username = username
            };

            MockGETRequestResult(userDataRequestByNameResult);

            var sut = new Mock<UserDataManager>(_apiClient);

            var mockUserData = new UserData(userDataRequestByNameResult.Id, username, new Dictionary<(string pieceId, string color), int> { { ("123", "45"), 1 } });

            sut.Setup(udm => udm.GetUserDataById(userDataRequestByNameResult.Id)).ReturnsAsync(mockUserData);

            // Act
            var result = await sut.Object.GetUserDataByName(username);

            // Assert
            result.Should().BeEquivalentTo(mockUserData);
        }

        [Fact]
        public async Task GetUserDataByName_ShouldReturnNullIfUserIsNotFound()
        {
            // Arrange
            var username = "TestUser";

            MockGETRequestResult<WithUsernameGetResponse?>(null);

            var sut = CreateManager();

            // Act
            var result = await sut.GetUserDataByName(username);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetUserDataById_ShouldReturnDetailedUserDataIfFound()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            var userDataRequestByIdResult = new ByIdGetResponse
            {
                Id = userId,
                Username = "TestUser",
                Location = "Location",
                BrickCount = 105,
                Collection = [
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
                ]
            };

            MockGETRequestResult(userDataRequestByIdResult);

            var sut = CreateManager();

            // Act
            var result = await sut.GetUserDataById(userId);

            // Assert
            var expectedResult = new UserData(userDataRequestByIdResult.Id, userDataRequestByIdResult.Username, new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "1"), 2 },
                { ("100", "2"), 3 },
                { ("300", "100"), 100 },
            });

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Fact]
        public async Task GetUserDataById_ShouldReturnNullIfUserIsNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();

            MockGETRequestResult<ByIdGetResponse?>(null);

            var sut = CreateManager();

            // Act
            var result = await sut.GetUserDataById(userId);

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
