using BrickApi.Client.Api.Sets;
using BuilderCatalogue.DTOs;
using BuilderCatalogue.Managers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BuilderCatalogue.Tests.Managers.SolutionsManagerTests
{
    public class FourthSolutionTests
    {
        private readonly Mock<ISetDataManager> _setDataManagerMock = new();
        private readonly Mock<IUserDataManager> _userDataManagerMock = new();
        private readonly Mock<ILogger<SolutionsManager>> _loggerMock = new();

        private SolutionsManager CreateManager() => new(_setDataManagerMock.Object, _userDataManagerMock.Object, _loggerMock.Object);

        [Fact]
        public async Task ExampleScenarioFromAssignmentDescription()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "dr_crocodile", new Dictionary<(string pieceId, string color), int>
            {
                { ("wall", "red"), 4 },
                { ("roof", "blue"), 1 },
                { ("flag", "green"), 1 },
            });

            var sets = new List<SetsGetResponse_Sets>() { new() { Id = Guid.NewGuid().ToString(), Name = "building" } };

            var setData = new SetData(sets[0].Id!, sets[0].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("wall", "white"), 4 },
                { ("roof", "red"), 1 },
                { ("flag", "green"), 1 },
            });

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);
            _setDataManagerMock.Setup(sdm => sdm.GetSetDataById(setData.Id)).ReturnsAsync(setData);

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFourthAssignment();

            // Assert
            result.Should().ContainSingle(setName => setName == setData.Name);
        }

        [Fact]
        public async Task OneOfMultipleSetsIsNewlyBuildable()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "dr_crocodile", new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "1"), 2 },
                { ("100", "2"), 3 },
                { ("300", "100"), 100 },
            });

            var setCount = 3;
            var sets = new List<SetsGetResponse_Sets>();
            for (var i = 0; i < setCount; i++)
            {
                sets.Add(new() { Id = Guid.NewGuid().ToString(), Name = $"set{i}" });
            }

            var setsData = new SetData[setCount];

            setsData[0] = new(sets[0].Id!, sets[0].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "100"), 2 },
                { ("100", "1"), 3 },
                { ("300", "2"), 100 },
            });
            setsData[1] = new(sets[1].Id!, sets[1].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("1", "1"), 1 },
            });
            setsData[2] = new(sets[2].Id!, sets[2].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("2", "2"), 2 },
            });

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);
            _setDataManagerMock.Setup(sdm => sdm.GetSetDataById(It.IsAny<string>())).ReturnsAsync((string id) => setsData.SingleOrDefault(sd => sd.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFourthAssignment();

            // Assert
            result.Should().ContainSingle(setName => setName == setsData[0].Name);
        }

        [Fact]
        public async Task PreviouslyBuildableSetIsNotNewlyBuildable()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "dr_crocodile", new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "1"), 2 },
                { ("100", "2"), 3 },
                { ("300", "100"), 100 },
            });

            var setCount = 3;
            var sets = new List<SetsGetResponse_Sets>();
            for (var i = 0; i < setCount; i++)
            {
                sets.Add(new() { Id = Guid.NewGuid().ToString(), Name = $"set{i}" });
            }

            var setsData = new SetData[setCount];

            setsData[0] = new(sets[0].Id!, sets[0].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "1"), 2 },
                { ("100", "2"), 3 },
                { ("300", "100"), 100 },
            });
            setsData[1] = new(sets[1].Id!, sets[1].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("1", "1"), 1 },
            });
            setsData[2] = new(sets[2].Id!, sets[2].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("2", "2"), 2 },
            });

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);
            _setDataManagerMock.Setup(sdm => sdm.GetSetDataById(It.IsAny<string>())).ReturnsAsync((string id) => setsData.SingleOrDefault(sd => sd.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFourthAssignment();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task MultipleSetsAreNewlyBuildable()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "dr_crocodile", new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "1"), 2 },
                { ("100", "2"), 3 },
                { ("300", "100"), 100 },
            });

            var setCount = 3;
            var sets = new List<SetsGetResponse_Sets>();
            for (var i = 0; i < setCount; i++)
            {
                sets.Add(new() { Id = Guid.NewGuid().ToString(), Name = $"set{i}" });
            }

            var setsData = new SetData[setCount];

            setsData[0] = new(sets[0].Id!, sets[0].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "100"), 2 },
                { ("100", "1"), 3 },
                { ("300", "2"), 100 },
            });
            setsData[1] = new(sets[1].Id!, sets[1].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "2"), 1 },
                { ("100", "100"), 2 },
                { ("300", "1"), 50 },
            });
            setsData[2] = new(sets[2].Id!, sets[2].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("2", "2"), 2 },
            });

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);
            _setDataManagerMock.Setup(sdm => sdm.GetSetDataById(It.IsAny<string>())).ReturnsAsync((string id) => setsData.SingleOrDefault(sd => sd.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFourthAssignment();

            // Assert
            result.Should().HaveCount(2).And.Contain([setsData[0].Name, setsData[1].Name]);
        }

        [Fact]
        public async Task SameColourCanNotBeUsedTwice()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "dr_crocodile", new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "1"), 2 },
                { ("100", "2"), 3 },
                { ("300", "2"), 100 },
            });

            var setCount = 3;
            var sets = new List<SetsGetResponse_Sets>();
            for (var i = 0; i < setCount; i++)
            {
                sets.Add(new() { Id = Guid.NewGuid().ToString(), Name = $"set{i}" });
            }

            var setsData = new SetData[setCount];

            setsData[0] = new(sets[0].Id!, sets[0].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "100"), 2 },
                { ("100", "1"), 3 },
                { ("300", "2"), 100 },
            });
            setsData[1] = new(sets[1].Id!, sets[1].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "2"), 1 },
                { ("100", "100"), 1 },
                { ("300", "1"), 1 },
            });
            setsData[2] = new(sets[2].Id!, sets[2].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("2", "2"), 2 },
            });

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);
            _setDataManagerMock.Setup(sdm => sdm.GetSetDataById(It.IsAny<string>())).ReturnsAsync((string id) => setsData.SingleOrDefault(sd => sd.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFourthAssignment();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task NoneOfTheSetsIsBuildable()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "dr_crocodile", new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "1"), 2 },
                { ("100", "2"), 3 },
                { ("300", "100"), 100 },
            });

            var setCount = 3;
            var sets = new List<SetsGetResponse_Sets>();
            for (var i = 0; i < setCount; i++)
            {
                sets.Add(new() { Id = Guid.NewGuid().ToString(), Name = $"set{i}" });
            }

            var setsData = new SetData[setCount];

            setsData[0] = new(sets[0].Id!, sets[0].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "100"), 2 },
                { ("300", "1"), 3 },
                { ("4", "2"), 100 },
            });
            setsData[1] = new(sets[1].Id!, sets[1].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("1", "1"), 1 },
            });
            setsData[2] = new(sets[2].Id!, sets[2].Name!, new Dictionary<(string pieceId, string color), int>
            {
                { ("2", "2"), 2 },
            });

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);
            _setDataManagerMock.Setup(sdm => sdm.GetSetDataById(It.IsAny<string>())).ReturnsAsync((string id) => setsData.SingleOrDefault(sd => sd.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFourthAssignment();

            // Assert
            result.Should().BeEmpty();
        }
        [Fact]
        public async Task EmptyListOfSets()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "dr_crocodile", new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "1"), 2 },
                { ("100", "2"), 3 },
                { ("300", "100"), 100 },
            });

            var sets = new List<SetsGetResponse_Sets>();

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFourthAssignment();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SetHasNoElements()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "dr_crocodile", new Dictionary<(string pieceId, string color), int>
            {
                { ("100", "1"), 2 },
                { ("100", "2"), 3 },
                { ("300", "100"), 100 },
            });

            var sets = new List<SetsGetResponse_Sets>() { new() { Id = Guid.NewGuid().ToString(), Name = "set" } };

            var setData = new SetData(sets[0].Id!, sets[0].Name!, []);

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);
            _setDataManagerMock.Setup(sdm => sdm.GetSetDataById(sets[0].Id!)).ReturnsAsync(setData);

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFourthAssignment();

            // Assert
            result.Should().BeEmpty();  // It was "buildable" before
        }
    }
}
