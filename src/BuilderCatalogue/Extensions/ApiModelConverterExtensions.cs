using BuilderCatalogue.DTOs;

namespace BuilderCatalogue.Extensions
{
    public static class ApiModelConverterExtensions
    {
        public static UserData? ToUserData(this BrickApi.Client.Api.User.ById.Item.ByIdGetResponse userResponse)
        {
            if (userResponse?.Id is null || userResponse.Username is null)
                return null;

            var userCollection = new Dictionary<(string pieceId, string color), int>();
            foreach (var piece in userResponse.Collection ?? [])
            {
                foreach (var variant in piece?.Variants ?? [])
                {
                    if (piece?.PieceId is null || variant?.Color is null || variant.Count is null)
                        continue;

                    userCollection[(piece.PieceId, variant.Color)] = (int)variant.Count;
                }
            }

            return new UserData(userResponse.Id, userResponse.Username, userCollection);
        }

        public static SetData? ToSetData(this BrickApi.Client.Api.Set.ById.Item.ByIdGetResponse setResponse)
        {
            if (setResponse.Id is null || setResponse.Name is null)
                return null;

            var setPieces = new Dictionary<(string pieceId, string color), int>();
            foreach (var piece in setResponse.Pieces ?? [])
            {
                if (piece?.Part?.DesignID is null || piece.Part.Material is null || piece.Quantity is null)
                    continue;

                setPieces[(piece.Part.DesignID, ((int)piece.Part.Material).ToString())] = (int)piece.Quantity;
            }

            return new SetData(setResponse.Id, setResponse.Name, setPieces);
        }
    }
}
