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

        public async Task<int> SolveThirdAssignment(string username = "megabuilder99")
        {
            var userData = await userDataManager.GetUserDataByName(username);

            if (userData is null)
                return 0;

            var allUsersData = await userDataManager.GetAllUsersDataWithDetails();
            var halfOfUsers = (int)Math.Floor((double)allUsersData.Count() / 2);

            var allPiecesWithCounts = GenerateListOfAllPiecesWithAllPossibleAmounts(allUsersData, userData);

            var pieceExcludesUsersCount = GetHowManyUsersEachPieceExcludes(allPiecesWithCounts, allUsersData);

            var table = new int[halfOfUsers, allPiecesWithCounts.Count];

            for (var i = pieceExcludesUsersCount[0]; i < halfOfUsers; i++)
                table[i, 0] = allPiecesWithCounts[0].Item2;

            for (var j = 0; j < halfOfUsers; j++)
            {
                for (var i = 1; i < allPiecesWithCounts.Count; i++)
                {
                    table[j, i] = table[j, i - 1];
                    if (pieceExcludesUsersCount[i] <= j)
                    {
                        var valueWithAddingNewItem = allPiecesWithCounts[i].Item1 != allPiecesWithCounts[i - 1].Item1
                                                        ? table[j - pieceExcludesUsersCount[i], i - 1] + allPiecesWithCounts[i].Item2
                                                        : table[j - pieceExcludesUsersCount[i], i - 1] - allPiecesWithCounts[i - 1].Item2 + allPiecesWithCounts[i].Item2;

                        table[j, i] = int.Max(table[j, i], valueWithAddingNewItem);
                    }
                }
            }

            var max = 0;
            for (var j = 0; j < halfOfUsers; j++)
            {
                for (var i = 0; i < allPiecesWithCounts.Count; i++)
                {
                    max = int.Max(table[j, i], max);
                }
            }

            return max;
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
                if (IsSetBuildableWithDifferentColours(setData, userData) && setData?.Name is not null)
                    buildableSets.Add(setData.Name);
            });

            var previouslyBuildableSets = await SolveFirstAssignment(username) ?? [];

            return buildableSets.Except(previouslyBuildableSets);
        }

        private static bool IsSetBuildableWithDifferentColours(SetData? setData, UserData? userData)
        {
            if (setData?.Pieces is null || userData?.Collection is null)
                return false;

            var possibleColourSubstitutes = new Dictionary<ColouredPiece, IEnumerable<string>>();
            foreach (var piece in setData.Pieces)
            {
                possibleColourSubstitutes[piece.Key] = userData.Collection.Where(p => p.Key.PieceId == piece.Key.PieceId && p.Value >= piece.Value).Select(p => p.Key.Colour).ToList();

                if (!possibleColourSubstitutes[piece.Key].Any())
                    return false;
            }

            var i = 0;
            var usedColours = new HashSet<string>();
            var usedElements = new Dictionary<ColouredPiece, int>();
            var elementsCount = setData.Pieces.Count;
            while (i >= 0 && i < elementsCount)
            {
                var currentElement = setData.Pieces.ElementAt(i).Key;
                var isInitialValuePresent = true;
                if (!usedElements.TryGetValue(currentElement, out int originalColourIndex))
                {
                    usedElements[currentElement] = 0;
                    originalColourIndex = usedElements[currentElement];
                    isInitialValuePresent = false;
                }

                if (UsePossibleSubstitutePieceIfExists(currentElement, usedElements, possibleColourSubstitutes, usedColours))
                {
                    usedColours.Add(possibleColourSubstitutes[currentElement].ElementAt(usedElements[currentElement]));
                    i++;
                }
                else
                {
                    if (isInitialValuePresent)
                        usedColours.Remove(possibleColourSubstitutes[currentElement].ElementAt(originalColourIndex));
                    usedElements.Remove(currentElement);
                    i--;
                }
            }

            if (i >= elementsCount)
                return true;

            return false;
        }

        private static bool UsePossibleSubstitutePieceIfExists(ColouredPiece currentElement,
                                    Dictionary<ColouredPiece, int> usedElements,
                                    Dictionary<ColouredPiece, IEnumerable<string>> possibleColourSubstitutes,
                                    HashSet<string> usedColours)
        {
            var possibleSubstitutesCount = possibleColourSubstitutes[currentElement].Count();
            while (usedElements[currentElement] < possibleSubstitutesCount && usedColours.Contains(possibleColourSubstitutes[currentElement].ElementAt(usedElements[currentElement])))
            {
                usedElements[currentElement]++;
            }
            return usedElements[currentElement] < possibleSubstitutesCount;
        }

        private static bool CanBuildSet(Dictionary<ColouredPiece, int> userCollection, Dictionary<ColouredPiece, int> setPieces)
            => !setPieces.Any(p => !userCollection.ContainsKey(p.Key) || userCollection[p.Key] < p.Value);

        private static List<(ColouredPiece, int)> GenerateListOfAllPiecesWithAllPossibleAmounts(IEnumerable<UserData> allUsersData, UserData userData)
        {
            var minPieces = new Dictionary<ColouredPiece, int>();
            var maxPieces = new Dictionary<ColouredPiece, int>();

            foreach (var userPieceWithColour in userData.Collection.Select(userPiece => userPiece.Key))
            {
                minPieces[userPieceWithColour] = 0;
                maxPieces[userPieceWithColour] = 0;
                foreach (var otherUserData in allUsersData)
                {
                    if (otherUserData.Collection.TryGetValue(userPieceWithColour, out int quantity))
                    {
                        if (quantity < minPieces[userPieceWithColour])
                            minPieces[userPieceWithColour] = quantity;

                        if (quantity > maxPieces[userPieceWithColour])
                            maxPieces[userPieceWithColour] = quantity;
                    }
                }
            }

            var allPieces = new List<(ColouredPiece, int)>();

            foreach (var userPieceWithColour in userData.Collection.Select(userPiece => userPiece.Key))
            {
                for (var i = minPieces[userPieceWithColour]; i <= maxPieces[userPieceWithColour]; i++)
                {
                    allPieces.Add((userPieceWithColour, i));
                }
            }

            return allPieces;
        }

        private static int[] GetHowManyUsersEachPieceExcludes(List<(ColouredPiece, int)>  allPiecesWithCounts, IEnumerable<UserData> allUsersData)
        {
            var pieceExcludesUsersCount = new int[allPiecesWithCounts.Count];
            for (var i = 0; i < allPiecesWithCounts.Count; i++)
            {
                pieceExcludesUsersCount[i] = allUsersData.Count();
                foreach (var otherUserData in allUsersData)
                {
                    if (otherUserData.Collection.TryGetValue(allPiecesWithCounts[i].Item1, out int quantity) && quantity >= allPiecesWithCounts[i].Item2)
                    {
                        pieceExcludesUsersCount[i]--;
                    }
                }
            }

            return pieceExcludesUsersCount;
        }
    }
}
