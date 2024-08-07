namespace BuilderCatalogue.DTOs
{
    public record SetData(string Id, string Name, Dictionary<ColouredPiece, int> Pieces);
}
