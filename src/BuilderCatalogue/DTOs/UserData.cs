namespace BuilderCatalogue.DTOs
{
    public record UserData(string Id, string Name, Dictionary<(string pieceId, string color), int> Collection);
}
