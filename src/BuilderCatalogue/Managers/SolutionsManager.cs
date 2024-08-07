using BuilderCatalogue.DTOs;

namespace BuilderCatalogue.Managers
{
    public class SolutionsManager(ISetDataManager setDataManager, IUserDataManager userDataManager, ILogger<SolutionsManager> logger) : ISolutionsManager
    {
        public async Task<IEnumerable<string>> SolveFirstAssignment(string username = "brickfan35")
        {
            var userData = await userDataManager.GetUserDataByName(username);

            if (userData is null)
                return [];

            var buildableSets = new List<string>();

            var sets = await setDataManager.GetAllSets();
            foreach (var set in sets)
            {
                if (set?.Id is null || set.Name is null)
                    continue;

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

        public async Task<IEnumerable<string>> SolveSecondAssignment(string username = "landscape-artist", string setName = "tropical-island")
        {
            var userData = await userDataManager.GetUserDataByName(username);
            var setData = await setDataManager.GetSetDataByName(setName);

            if (userData is null)
                return [];

            if (setData is null)
                return [];

            var missingElements = new Dictionary<ColouredPiece, int>();
            foreach (var setPiece in setData.Pieces)
            {
                if (!userData.Collection.TryGetValue(setPiece.Key, out int userCollectionPieceAmount))
                {
                    missingElements[setPiece.Key] = setPiece.Value;
                }
                else if (userCollectionPieceAmount < setPiece.Value)
                {
                    missingElements[setPiece.Key] = setPiece.Value - userCollectionPieceAmount;
                }
            }

            var usersToCollaborate = new List<string>();

            var users = await userDataManager.GetAllUsers();
            foreach (var user in users)
            {
                if (user?.Id is null || user.Username is null || user.Id == userData.Id)
                    continue;

                var otherUserData = await userDataManager.GetUserDataById(user.Id);
                if (CanBuildSet(otherUserData?.Collection ?? [], missingElements))
                {
                    usersToCollaborate.Add(user.Username);
                }
            }

            return usersToCollaborate;
        }

        public int SolveThirdAssignment()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<string>> SolveFourthAssignment(string username = "dr_crocodile")
        {
            var userData = await userDataManager.GetUserDataByName(username);

            var sets = await setDataManager.GetAllSets();

            var buildableSets = new List<string>();

            await Parallel.ForEachAsync(sets, async (set, _) =>
            {
                if (set?.Id is null)
                    return;

                var setData = await setDataManager.GetSetDataById(set.Id);
                if (IsSetBuildableWithDifferentColors(setData, userData) && setData?.Name is not null)
                    buildableSets.Add(setData.Name);
            });

            var previouslyBuildableSets = await SolveFirstAssignment(username) ?? [];

            return buildableSets.Except(previouslyBuildableSets);
        }

        private static bool IsSetBuildableWithDifferentColors(SetData? setData, UserData? userData)
        {
            if (setData?.Pieces is null || userData?.Collection is null)
                return false;

            var possibleColorSubstitutes = new Dictionary<ColouredPiece, IEnumerable<string>>();
            foreach (var piece in setData.Pieces)
            {
                possibleColorSubstitutes[piece.Key] = userData.Collection.Where(p => p.Key.PieceId == piece.Key.PieceId && p.Value >= piece.Value).Select(p => p.Key.Colour).ToList();

                if (!possibleColorSubstitutes[piece.Key].Any())
                    return false;
            }

            var i = 0;
            var usedColors = new HashSet<string>();
            var usedElements = new Dictionary<ColouredPiece, int>();
            var elementsCount = setData.Pieces.Count;
            while (i >= 0 && i < elementsCount)
            {
                var currentElement = setData.Pieces.ElementAt(i).Key;
                var isInitialValuePresent = true;
                if (!usedElements.TryGetValue(currentElement, out int originalColorIndex))
                {
                    usedElements[currentElement] = 0;
                    originalColorIndex = usedElements[currentElement];
                    isInitialValuePresent = false;
                }

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

        private static bool PossibleSubstitutePieceExists(ColouredPiece currentElement,
                                    Dictionary<ColouredPiece, int> usedElements,
                                    Dictionary<ColouredPiece, IEnumerable<string>> possibleColorSubstitutes,
                                    HashSet<string> usedColors)
        {
            var possibleSubstitutesCount = possibleColorSubstitutes[currentElement].Count();
            while (usedElements[currentElement] < possibleSubstitutesCount && usedColors.Contains(possibleColorSubstitutes[currentElement].ElementAt(usedElements[currentElement])))
            {
                usedElements[currentElement]++;
            }
            return usedElements[currentElement] < possibleSubstitutesCount;
        }

        private static bool CanBuildSet(Dictionary<ColouredPiece, int> userCollection, Dictionary<ColouredPiece, int> setPieces)
            => !setPieces.Any(p => !userCollection.ContainsKey(p.Key) || userCollection[p.Key] < p.Value);
    }
}
