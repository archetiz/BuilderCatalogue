using BuilderCatalogue.DTOs;

namespace BuilderCatalogue.Managers
{
    public class SolutionsManager(ISetDataManager setDataManager, IUserDataManager userDataManager, ILogger<SolutionsManager> logger) : ISolutionsManager
    {
        public async Task<IEnumerable<string>> SolveFirstAssignment()
        {
            var user = "brickfan35";
            var userData = await userDataManager.GetUserDataByName(user);

            if (userData is null)
                return [];  // TODO: handle error

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
                return [];  // TODO: handle error

            if (setData is null)
                //return Results.NotFound("Set not found");
                return [];  // TODO: handle error

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
            var username = "dr_crocodile";
            var userData = await userDataManager.GetUserDataByName(username);
            var sets = await setDataManager.GetAllSets();

            var buildableSets = new List<string>();

            await Parallel.ForEachAsync(sets, async (set, _) =>
            {
                var setData = await setDataManager.GetSetDataById(set.Id);
                if (IsSetBuildableWithDifferentColors(setData, userData))
                    buildableSets.Add(setData.Name);
            });

            return buildableSets;
        }

        private static bool IsSetBuildableWithDifferentColors(SetData setData, UserData userData)
        {
            var possibleColorSubstitutes = new Dictionary<(string pieceId, string color), IEnumerable<string>>();
            foreach (var piece in setData.Pieces)
            {
                possibleColorSubstitutes[piece.Key] = userData.Collection.Where(p => p.Key.pieceId == piece.Key.pieceId && p.Value >= piece.Value).Select(p => p.Key.color).ToList();

                if (possibleColorSubstitutes[piece.Key].Count() == 0)
                    return false;
            }

            var i = 0;
            var usedColors = new HashSet<string>();
            var usedElements = new Dictionary<(string pieceId, string color), int>();
            var elementsCount = setData.Pieces.Count;
            while (i >= 0 && i < elementsCount)
            {
                var currentElement = setData.Pieces.ElementAt(i).Key;
                var isInitialValuePresent = true;
                if (!usedElements.ContainsKey(currentElement))
                {
                    usedElements[currentElement] = 0;
                    isInitialValuePresent = false;
                }
                var originalColorIndex = usedElements[currentElement];
                if (PossibleSubstitutePieceExists(currentElement, usedElements, possibleColorSubstitutes, usedColors))
                {
                    usedColors.Add(possibleColorSubstitutes[currentElement].ElementAt(usedElements[currentElement]));
                    i++;
                }
                else
                {
                    if (isInitialValuePresent)
                        usedColors.Remove(possibleColorSubstitutes[currentElement].ElementAt(originalColorIndex));
                    usedElements.Remove(currentElement);
                    i--;
                }
            }

            if (i >= elementsCount)
                return true;

            return false;
        }

        private static bool PossibleSubstitutePieceExists((string pieceId, string color) currentElement,
                                    Dictionary<(string pieceId, string color), int> usedElements,
                                    Dictionary<(string pieceId, string color), IEnumerable<string>> possibleColorSubstitutes,
                                    HashSet<string> usedColors)
        {
            var possibleSubstitutesCount = possibleColorSubstitutes[currentElement].Count();
            while (usedElements[currentElement] < possibleSubstitutesCount && usedColors.Contains(possibleColorSubstitutes[currentElement].ElementAt(usedElements[currentElement])))
            {
                usedElements[currentElement]++;
            }
            return usedElements[currentElement] < possibleSubstitutesCount;
        }

        private static bool CanBuildSet(Dictionary<(string pieceId, string color), int> userCollection, Dictionary<(string pieceId, string color), int> setPieces)
            => !setPieces.Any(p => !userCollection.ContainsKey(p.Key) || userCollection[p.Key] < p.Value);
    }
}
