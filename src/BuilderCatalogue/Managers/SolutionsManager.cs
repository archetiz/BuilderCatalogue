namespace BuilderCatalogue.Managers
{
    public class SolutionsManager(ISetDataManager setDataManager, IUserDataManager userDataManager, ILogger<SolutionsManager> logger) : ISolutionsManager
    {
        public async Task<IEnumerable<string>> SolveFirstAssignment()
        {
            var user = "brickfan35";
            var userData = await userDataManager.GetUserDataByName(user);

            if (userData is null)
                return Enumerable.Empty<string>();  // TODO: handle error

            var buildableSets = new List<string>();

            var sets = await setDataManager.GetAllSets();
            foreach (var set in sets)
            {
                var setData = await setDataManager.GetSetDataById(set.Id);
                if (setData is null)
                {
                    logger.LogWarning("Set with ID {Id} not found", set.Id);
                }
                else if (CanBuildSet(userData.Collection, setData.Pieces))
                {
                    buildableSets.Add(set.Name);
                }
            }

            return buildableSets;
        }

        public async Task<IEnumerable<string>> SolveSecondAssignment()
        {
            var username = "landscape-artist";
            var set = "tropical-island";

            var userData = await userDataManager.GetUserDataByName(username);
            var setData = await setDataManager.GetSetDataByName(set);

            if (userData is null)
                //return Results.NotFound("User not found");
                return Enumerable.Empty<string>();  // TODO: handle error

            if (setData is null)
                //return Results.NotFound("Set not found");
                return Enumerable.Empty<string>();  // TODO: handle error

            var missingElements = new Dictionary<(string pieceId, string color), int>();
            foreach (var setPiece in setData.Pieces)
            {
                if (!userData.Collection.ContainsKey(setPiece.Key))
                {
                    missingElements[setPiece.Key] = setPiece.Value;
                }
                else if (userData.Collection[setPiece.Key] < setPiece.Value)
                {
                    missingElements[setPiece.Key] = userData.Collection[setPiece.Key] - setPiece.Value;
                }
            }

            var usersToCollaborate = new List<string>();

            var users = await userDataManager.GetAllUsers();
            foreach (var user in users)
            {
                if (user.Id == userData.Id)
                    continue;

                var otherUserData = await userDataManager.GetUserDataById(user.Id);
                if (CanBuildSet(otherUserData.Collection, missingElements))
                {
                    usersToCollaborate.Add(user.Username);
                }
            }

            return usersToCollaborate;
        }

        public async Task<int> SolveThirdAssignment()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> SolveFourthAssignment()
        {
            throw new NotImplementedException();
        }

        private static bool CanBuildSet(Dictionary<(string pieceId, string color), int> userCollection, Dictionary<(string pieceId, string color), int> setPieces)
            => !setPieces.Any(p => !userCollection.ContainsKey(p.Key) || userCollection[p.Key] < p.Value);
    }
}
