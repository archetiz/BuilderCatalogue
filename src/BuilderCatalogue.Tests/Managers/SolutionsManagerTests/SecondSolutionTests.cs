using BrickApi.Client.Api.Users;
using BuilderCatalogue.DTOs;
using BuilderCatalogue.Managers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BuilderCatalogue.Tests.Managers.SolutionsManagerTests
{
    public class SecondSolutionTests
    {
        private readonly Mock<ISetDataManager> _setDataManagerMock = new();
        private readonly Mock<IUserDataManager> _userDataManagerMock = new();
        private readonly Mock<ILogger<SolutionsManager>> _loggerMock = new();

        private SolutionsManager CreateManager() => new(_setDataManagerMock.Object, _userDataManagerMock.Object, _loggerMock.Object);

        [Fact]
        public async Task CanCollaborateWithASingleUser()
        {
            // Arrange
            var username = "landscape-artist";
            var setName = "tropical-island";

            var userData = new UserData(Guid.NewGuid().ToString(), username, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });

            var setData = new SetData(Guid.NewGuid().ToString(), setName, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("100", "1"), 2 },
                { new("300", "100"), 50 },
                { new("4", "2"), 3 },
            });

            var usersCount = 3;
            var users = new List<UsersGetResponse_Users>();
            for (var i = 0; i < usersCount; i++)
            {
                users.Add(new() { Id = Guid.NewGuid().ToString(), Username = $"user{i}" });
            }

            var usersData = new UserData[usersCount];

            usersData[0] = new(users[0].Id!, users[0].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
            });
            usersData[1] = new(users[1].Id!, users[1].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("2", "1"), 42 },
            });
            usersData[2] = new(users[2].Id!, users[2].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("4", "2"), 3 },
                { new("300", "100"), 20 },
            });

            _setDataManagerMock.Setup(sdm => sdm.GetSetDataByName(setName)).ReturnsAsync(setData);
            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _userDataManagerMock.Setup(udm => udm.GetAllUsers()).ReturnsAsync(users);
            _userDataManagerMock.Setup(sdm => sdm.GetUserDataById(It.IsAny<string>())).ReturnsAsync((string id) => usersData.SingleOrDefault(ud => ud.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveSecondAssignment();

            // Assert
            result.Should().ContainSingle(username => username == usersData[2].Name);
        }

        [Fact]
        public async Task CanCollaborateWithMultipleUsers()
        {
            // Arrange
            var username = "landscape-artist";
            var setName = "tropical-island";

            var userData = new UserData(Guid.NewGuid().ToString(), username, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });

            var setData = new SetData(Guid.NewGuid().ToString(), setName, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("100", "1"), 2 },
                { new("300", "100"), 50 },
                { new("4", "2"), 3 },
            });

            var usersCount = 3;
            var users = new List<UsersGetResponse_Users>();
            for (var i = 0; i < usersCount; i++)
            {
                users.Add(new() { Id = Guid.NewGuid().ToString(), Username = $"user{i}" });
            }

            var usersData = new UserData[usersCount];

            usersData[0] = new(users[0].Id!, users[0].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("4", "2"), 4 },
            });
            usersData[1] = new(users[1].Id!, users[1].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("2", "1"), 42 },
            });
            usersData[2] = new(users[2].Id!, users[2].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("4", "2"), 3 },
                { new("5", "100"), 10 },
            });

            _setDataManagerMock.Setup(sdm => sdm.GetSetDataByName(setName)).ReturnsAsync(setData);
            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _userDataManagerMock.Setup(udm => udm.GetAllUsers()).ReturnsAsync(users);
            _userDataManagerMock.Setup(sdm => sdm.GetUserDataById(It.IsAny<string>())).ReturnsAsync((string id) => usersData.SingleOrDefault(ud => ud.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveSecondAssignment();

            // Assert
            result.Should().HaveCount(2).And.Contain([usersData[0].Name, usersData[2].Name]);
        }


        [Fact]
        public async Task CanNotCollaborateWithAnyone()
        {
            // Arrange
            var username = "landscape-artist";
            var setName = "tropical-island";

            var userData = new UserData(Guid.NewGuid().ToString(), username, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });

            var setData = new SetData(Guid.NewGuid().ToString(), setName, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("100", "1"), 2 },
                { new("300", "100"), 50 },
                { new("4", "2"), 3 },
            });

            var usersCount = 3;
            var users = new List<UsersGetResponse_Users>();
            for (var i = 0; i < usersCount; i++)
            {
                users.Add(new() { Id = Guid.NewGuid().ToString(), Username = $"user{i}" });
            }

            var usersData = new UserData[usersCount];

            usersData[0] = new(users[0].Id!, users[0].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "2"), 1 },
                { new("4", "4"), 4 },
            });
            usersData[1] = new(users[1].Id!, users[1].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("2", "1"), 42 },
            });
            usersData[2] = new(users[2].Id!, users[2].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("5", "100"), 10 },
            });

            _setDataManagerMock.Setup(sdm => sdm.GetSetDataByName(setName)).ReturnsAsync(setData);
            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _userDataManagerMock.Setup(udm => udm.GetAllUsers()).ReturnsAsync(users);
            _userDataManagerMock.Setup(sdm => sdm.GetUserDataById(It.IsAny<string>())).ReturnsAsync((string id) => usersData.SingleOrDefault(ud => ud.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveSecondAssignment();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task UserHasNoPiecesAndNooneElseHasEnoughForFullSet()
        {
            // Arrange
            var username = "landscape-artist";
            var setName = "tropical-island";

            var userData = new UserData(Guid.NewGuid().ToString(), username, []);

            var setData = new SetData(Guid.NewGuid().ToString(), setName, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("100", "1"), 2 },
                { new("300", "100"), 50 },
                { new("4", "2"), 3 },
            });

            var usersCount = 3;
            var users = new List<UsersGetResponse_Users>();
            for (var i = 0; i < usersCount; i++)
            {
                users.Add(new() { Id = Guid.NewGuid().ToString(), Username = $"user{i}" });
            }

            var usersData = new UserData[usersCount];

            usersData[0] = new(users[0].Id!, users[0].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "2"), 1 },
                { new("4", "4"), 4 },
            });
            usersData[1] = new(users[1].Id!, users[1].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("2", "1"), 42 },
            });
            usersData[2] = new(users[2].Id!, users[2].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("5", "100"), 10 },
            });

            _setDataManagerMock.Setup(sdm => sdm.GetSetDataByName(setName)).ReturnsAsync(setData);
            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _userDataManagerMock.Setup(udm => udm.GetAllUsers()).ReturnsAsync(users);
            _userDataManagerMock.Setup(sdm => sdm.GetUserDataById(It.IsAny<string>())).ReturnsAsync((string id) => usersData.SingleOrDefault(ud => ud.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveSecondAssignment();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task UserHasNoPiecesButSomeoneElseHasEnoughToBuildTheFullSet()
        {
            // Arrange
            var username = "landscape-artist";
            var setName = "tropical-island";

            var userData = new UserData(Guid.NewGuid().ToString(), username, []);

            var setData = new SetData(Guid.NewGuid().ToString(), setName, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("100", "1"), 2 },
                { new("300", "100"), 50 },
                { new("4", "2"), 3 },
            });

            var usersCount = 3;
            var users = new List<UsersGetResponse_Users>();
            for (var i = 0; i < usersCount; i++)
            {
                users.Add(new() { Id = Guid.NewGuid().ToString(), Username = $"user{i}" });
            }

            var usersData = new UserData[usersCount];

            usersData[0] = new(users[0].Id!, users[0].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("100", "1"), 2 },
                { new("300", "100"), 50 },
                { new("4", "2"), 3 },
            });
            usersData[1] = new(users[1].Id!, users[1].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("2", "1"), 42 },
            });
            usersData[2] = new(users[2].Id!, users[2].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("5", "100"), 10 },
            });

            _setDataManagerMock.Setup(sdm => sdm.GetSetDataByName(setName)).ReturnsAsync(setData);
            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _userDataManagerMock.Setup(udm => udm.GetAllUsers()).ReturnsAsync(users);
            _userDataManagerMock.Setup(sdm => sdm.GetUserDataById(It.IsAny<string>())).ReturnsAsync((string id) => usersData.SingleOrDefault(ud => ud.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveSecondAssignment();

            // Assert
            result.Should().ContainSingle(username => username == usersData[0].Name);
        }

        [Fact]
        public async Task SetHasNoElements()
        {
            // Arrange
            var username = "landscape-artist";
            var setName = "tropical-island";

            var userData = new UserData(Guid.NewGuid().ToString(), username, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });

            var setData = new SetData(Guid.NewGuid().ToString(), setName, []);

            var usersCount = 3;
            var users = new List<UsersGetResponse_Users>();
            for (var i = 0; i < usersCount; i++)
            {
                users.Add(new() { Id = Guid.NewGuid().ToString(), Username = $"user{i}" });
            }

            var usersData = new UserData[usersCount];

            usersData[0] = new(users[0].Id!, users[0].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("100", "1"), 2 },
                { new("300", "100"), 50 },
                { new("4", "2"), 3 },
            });
            usersData[1] = new(users[1].Id!, users[1].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("2", "1"), 42 },
            });
            usersData[2] = new(users[2].Id!, users[2].Username!, new Dictionary<ColouredPiece, int>
            {
                { new("1", "1"), 1 },
                { new("5", "100"), 10 },
            });

            _setDataManagerMock.Setup(sdm => sdm.GetSetDataByName(setName)).ReturnsAsync(setData);
            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _userDataManagerMock.Setup(udm => udm.GetAllUsers()).ReturnsAsync(users);
            _userDataManagerMock.Setup(sdm => sdm.GetUserDataById(It.IsAny<string>())).ReturnsAsync((string id) => usersData.SingleOrDefault(ud => ud.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveSecondAssignment();

            // Assert
            result.Should().HaveCount(3).And.Contain(usersData.Select(ud => ud.Name));
        }
    }
}
