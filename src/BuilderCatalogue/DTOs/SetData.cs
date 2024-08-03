namespace BuilderCatalogue.DTOs
{
    public record SetData(string Id, string Name, Dictionary<(string pieceId, string color), int> Pieces);
}
