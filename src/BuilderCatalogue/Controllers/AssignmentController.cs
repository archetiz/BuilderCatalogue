using BrickApi.Client;
using BuilderCatalogue.DTOs;
using BuilderCatalogue.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace BuilderCatalogue.Controllers
{
    [ApiController]
    [Route("api/assigment")]
    public class AssignmentController(BrickApiClient apiClient, ILogger<AssignmentController> logger) : ControllerBase
    {
        [HttpGet("one")]
        public async Task<ActionResult<IEnumerable<string>>> SolveFirstAssignment()
        {
            var user = "brickfan35";
            var userData = await GetUserDataByName(user);

            if (userData is null)
                return NotFound("User not found");

            var buildableSets = new List<string>();

            var sets = await apiClient.Api.Sets.GetAsync();
            foreach (var set in sets.Sets)
            {
                var setData = await GetSetDataById(set.Id);
                if (setData is null)
                {
                    logger.LogWarning("Set with ID {Id} not found", set.Id);
                }
                else if (CanBuildSet(userData.Collection, setData.Pieces))
                {
                    buildableSets.Add(set.Name);
                }
            }

            return Ok(buildableSets);
        }

        private async Task<UserData?> GetUserDataByName(string username)
        {
            var userSummary = await apiClient.Api.User.ByUsername[username].GetAsync();
            if (userSummary?.Id is null)
                return null;

            return await GetUserDataById(userSummary.Id);
        }

        private async Task<UserData?> GetUserDataById(string id)
        {
            var userDataById = await apiClient.Api.User.ById[id].GetAsync();
            return userDataById?.ToUserData();
        }

        private async Task<SetData?> GetSetDataById(string id)
        {
            var setDataById = await apiClient.Api.Set.ById[id].GetAsync();
            return setDataById?.ToSetData();
        }

        private bool CanBuildSet(Dictionary<(string pieceId, string color), int> userCollection, Dictionary<(string pieceId, string color), int> setPieces)
            => !setPieces.Any(p => !userCollection.ContainsKey(p.Key) || userCollection[p.Key] < p.Value);

        private bool CanBuildSet(Dictionary<(string pieceId, string color), int> userCollection, BrickApi.Client.Api.Set.ById.Item.ByIdGetResponse setDetails)
        {
            foreach (var piece in setDetails.Pieces)
            {
                var key = (piece.Part.DesignID, ((int)piece.Part.Material).ToString());
                if (!userCollection.ContainsKey(key) || userCollection[key] < piece.Quantity)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CanBuildSet(Dictionary<(string pieceId, string color), int> setElements, BrickApi.Client.Api.User.ById.Item.ByIdGetResponse userDetails)
        {
            var userCollection = new Dictionary<(string pieceId, string color), int>();
            foreach (var piece in userDetails!.Collection)
            {
                foreach (var variant in piece.Variants)
                {
                    userCollection[(piece.PieceId, variant.Color)] = (int)variant.Count;
                }
            }

            foreach (var element in setElements)
            {
                if (!userCollection.ContainsKey(element.Key) || userCollection[element.Key] < element.Value)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
