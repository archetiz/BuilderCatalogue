using BrickApi.Client.Api.Sets;
using BuilderCatalogue.DTOs;
using BuilderCatalogue.Managers;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace BuilderCatalogue.Tests.Managers.SolutionsManagerTests
{
    public class FirstSolutionTests
    {
        private readonly Mock<ISetDataManager> _setDataManagerMock = new();
        private readonly Mock<IUserDataManager> _userDataManagerMock = new();
        private readonly Mock<ILogger<SolutionsManager>> _loggerMock = new();

        private SolutionsManager CreateManager() => new(_setDataManagerMock.Object, _userDataManagerMock.Object, _loggerMock.Object);

        [Fact]
        public async Task OneOfMultipleSetsIsBuildable()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "brickfan35", new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });

            var setCount = 3;
            var sets = new List<SetsGetResponse_Sets>();
            for (var i = 0; i < setCount; i++)
            {
                sets.Add(new() { Id = Guid.NewGuid().ToString(), Name = $"set{i}" });
            }

            var setsData = new SetData[setCount];

            setsData[0] = new(sets[0].Id!, sets[0].Name!, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 3 },
            });
            setsData[1] = new(sets[1].Id!, sets[1].Name!, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });
            setsData[2] = new(sets[2].Id!, sets[2].Name!, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 1 },
                { new("100", "2"), 2 },
                { new("300", "100"), 101 },
            });

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);
            _setDataManagerMock.Setup(sdm => sdm.GetSetDataById(It.IsAny<string>())).ReturnsAsync((string id) => setsData.SingleOrDefault(sd => sd.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFirstAssignment();

            // Assert
            result.Should().ContainSingle(setName => setName == setsData[1].Name);
        }

        [Fact]
        public async Task MultipleSetsAreBuildable()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "brickfan35", new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });

            var setCount = 3;
            var sets = new List<SetsGetResponse_Sets>();
            for (var i = 0; i < setCount; i++)
            {
                sets.Add(new() { Id = Guid.NewGuid().ToString(), Name = $"set{i}" });
            }

            var setData = new SetData[setCount];

            setData[0] = new(sets[0].Id!, sets[0].Name!, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 1 },
            });
            setData[1] = new(sets[1].Id!, sets[1].Name!, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });
            setData[2] = new(sets[2].Id!, sets[2].Name!, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 1 },
                { new("100", "2"), 2 },
                { new("300", "100"), 101 },
            });

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);
            _setDataManagerMock.Setup(sdm => sdm.GetSetDataById(It.IsAny<string>())).ReturnsAsync((string id) => setData.SingleOrDefault(sd => sd.Id == id));

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFirstAssignment();

            // Assert
            result.Should().HaveCount(2).And.Contain([setData[0].Name, setData[1].Name]);
        }

        [Fact]
        public async Task NoneOfTheSetsIsBuildable()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "brickfan35", new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });

            var sets = new List<SetsGetResponse_Sets>() { new() { Id = Guid.NewGuid().ToString(), Name = "set" } };

            var setData = new SetData(sets[0].Id!, sets[0].Name!, new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 3 },
            });

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);
            _setDataManagerMock.Setup(sdm => sdm.GetSetDataById(sets[0].Id!)).ReturnsAsync(setData);

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFirstAssignment();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task EmptyListOfSets()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "brickfan35", new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync([]);

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFirstAssignment();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task SetHasNoElements()
        {
            // Arrange
            var userData = new UserData(Guid.NewGuid().ToString(), "brickfan35", new Dictionary<ColouredPiece, int>
            {
                { new("100", "1"), 2 },
                { new("100", "2"), 3 },
                { new("300", "100"), 100 },
            });

            var sets = new List<SetsGetResponse_Sets>() { new() { Id = Guid.NewGuid().ToString(), Name = "set" } };

            var setData = new SetData(sets[0].Id!, sets[0].Name!, []);

            _userDataManagerMock.Setup(udm => udm.GetUserDataByName(It.IsAny<string>())).ReturnsAsync(userData);
            _setDataManagerMock.Setup(sdm => sdm.GetAllSets()).ReturnsAsync(sets);
            _setDataManagerMock.Setup(sdm => sdm.GetSetDataById(sets[0].Id!)).ReturnsAsync(setData);

            var sut = CreateManager();

            // Act
            var result = await sut.SolveFirstAssignment();

            // Assert
            result.Should().ContainSingle(setName => setName == setData.Name);
        }
    }
}
