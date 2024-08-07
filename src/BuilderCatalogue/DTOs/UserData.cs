namespace BuilderCatalogue.DTOs
{
    public record UserData(string Id, string Name, Dictionary<ColouredPiece, int> Collection);
}
