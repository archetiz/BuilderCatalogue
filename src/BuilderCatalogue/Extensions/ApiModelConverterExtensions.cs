using BuilderCatalogue.DTOs;

namespace BuilderCatalogue.Extensions
{
    public static class ApiModelConverterExtensions
    {
        public static UserData ToUserData(this BrickApi.Client.Api.User.ById.Item.ByIdGetResponse userResponse)
        {
            var userCollection = new Dictionary<(string pieceId, string color), int>();
            foreach (var piece in userResponse.Collection)
            {
                foreach (var variant in piece.Variants)
                {
                    userCollection[(piece.PieceId, variant.Color)] = (int)variant.Count;
                }
            }

            return new UserData(userResponse.Id, userResponse.Username, userCollection);
        }

        public static SetData ToSetData(this BrickApi.Client.Api.Set.ById.Item.ByIdGetResponse setResponse)
        {
            var setPieces = new Dictionary<(string pieceId, string color), int>();
            setResponse.Pieces.ForEach(p => setPieces[(p.Part.DesignID, ((int)p.Part.Material).ToString())] = (int)p.Quantity);

            return new SetData(setResponse.Id, setResponse.Name, setPieces);
        }
    }
}
