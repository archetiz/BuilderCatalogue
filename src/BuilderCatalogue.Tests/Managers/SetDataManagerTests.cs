using AutoFixture.Xunit2;
using BrickApi.Client;
using BrickApi.Client.Api.Set.ById.Item;
using BrickApi.Client.Api.Set.ByName.Item;
using BrickApi.Client.Api.Sets;
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
    public class SetDataManagerTests
    {
        private readonly Mock<IRequestAdapter> _requestAdapterMock = new();
        private readonly BrickApiClient _apiClient;

        public SetDataManagerTests()
        {
            _apiClient = new BrickApiClient(_requestAdapterMock.Object);
        }

        private SetDataManager CreateManager() => new(_apiClient);

        // Note: wanted to use AutoData everywhere (where possible) in this class, but some of the manually written test data here ended up too nicely,
        // and it would've made me sad to delete it, so decided to keep it, since this is only a demo project anyway

        [Fact]
        public async Task GetAllSets_ReturnsFullListNormally()
        {
            // Arrange
            var setsMock = new SetsGetResponse
            {
                Sets =
                [
                    new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        SetNumber = "123",
                        Name = "Best set ever",
                        TotalPieces = 456
                    },
                    new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        SetNumber = "42",
                        Name = "Ok, maybe there is a better one",
                        TotalPieces = 42
                    },
                ]
            };

            MockGETRequestResult(setsMock);

            var sut = CreateManager();

            // Act
            var result = await sut.GetAllSets();

            // Assert
            result.Should().Equal(setsMock.Sets);
        }


        [Fact]
        public async Task GetAllSets_ReturnsEmptyListWhenResultIsNullOrNotFound()
        {
            // Arrange
            MockGETRequestResult<SetsGetResponse?>(null);

            var sut = CreateManager();

            // Act
            var result = await sut.GetAllSets();

            // Assert
            result.Should().BeEmpty();
        }

        [Fact]
        public async Task GetSetDataByName_ShouldReturnDetailedSetDataIfFound()
        {
            // Arrange
            var setname = "It must be the real best set this time";

            var setDataRequestByNameResult = new WithNameGetResponse
            {
                Id = Guid.NewGuid().ToString(),
                SetNumber = "43",
                Name = setname,
                TotalPieces = 54321
            };

            MockGETRequestResult(setDataRequestByNameResult);

            var sut = new Mock<SetDataManager>(_apiClient);

            var mockSetData = new SetData(setDataRequestByNameResult.Id, setname, new Dictionary<ColouredPiece, int> { { new("123", "45"), 6 } });

            sut.Setup(udm => udm.GetSetDataById(setDataRequestByNameResult.Id)).ReturnsAsync(mockSetData);

            // Act
            var result = await sut.Object.GetSetDataByName(setname);

            // Assert
            result.Should().BeEquivalentTo(mockSetData);
        }

        [Theory, AutoData]
        public async Task GetSetDataByName_ShouldReturnNullIfSetIsNotFound(string setName)
        {
            // Arrange
            MockGETRequestResult<WithNameGetResponse?>(null);

            var sut = CreateManager();

            // Act
            var result = await sut.GetSetDataByName(setName);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetSetDataById_ShouldReturnDetailedSetDataIfFound()
        {
            // Arrange
            var setId = Guid.NewGuid().ToString();

            var setDataRequestByIdResult = new ByIdGetResponse
            {
                Id = setId,
                SetNumber = "44",
                Name = "Another best set...it's starting to be competition",
                TotalPieces = 55,
                Pieces = [
                    new() {
                       Part = new() {
                         DesignID = "0",
                         Material = 12,
                         PartType = "rigid"
                       },
                       Quantity = 0,
                    },
                    new() {
                       Part = new() {
                         DesignID = "1",
                         Material = 21,
                         PartType = "rigid"
                       },
                       Quantity = 1,
                    },
                    new() {
                       Part = new() {
                         DesignID = "2",
                         Material = 21,
                         PartType = "also...rigid"
                       },
                       Quantity = 4,
                    }
                ]
            };

            MockGETRequestResult(setDataRequestByIdResult);

            var sut = CreateManager();

            // Act
            var result = await sut.GetSetDataById(setId);

            // Assert
            var expectedResult = new SetData(setDataRequestByIdResult.Id, setDataRequestByIdResult.Name, new Dictionary<ColouredPiece, int>
            {
                { new("0", "12"), 0 },
                { new("1", "21"), 1 },
                { new("2", "21"), 4 },
            });

            result.Should().BeEquivalentTo(expectedResult);
        }

        [Theory, AutoData]
        public async Task GetSetDataById_ShouldReturnNullIfSetIsNotFound(Guid setId)
        {
            // Arrange
            MockGETRequestResult<ByIdGetResponse?>(null);

            var sut = CreateManager();

            // Act
            var result = await sut.GetSetDataById(setId.ToString());

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
