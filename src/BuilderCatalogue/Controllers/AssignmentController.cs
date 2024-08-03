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

        [HttpGet("two")]
        public async Task<ActionResult<IEnumerable<string>>> SolveSecondAssignment()
        {
            var username = "landscape-artist";
            var set = "tropical-island";

            var userData = await GetUserDataByName(username);
            var setData = await GetSetDataByName(set);

            if (userData is null)
                return NotFound("User not found");

            if (setData is null)
                return NotFound("Set not found");

            var missingElements = new Dictionary<(string pieceId, string color), int>();
            foreach (var sp in setData.Pieces)
            {
                if (!userData.Collection.ContainsKey(sp.Key))
                {
                    missingElements[sp.Key] = sp.Value;
                }
                else if (userData.Collection[sp.Key] < sp.Value)
                {
                    missingElements[sp.Key] = userData.Collection[sp.Key] - sp.Value;
                }
            }

            var usersToCollaborate = new List<string>();

            var users = await apiClient.Api.Users.GetAsync();
            foreach (var user in users.Users)
            {
                if (user.Id == userData.Id)
                    continue;

                var otherUserData = await GetUserDataById(user.Id);
                if (CanBuildSet(otherUserData.Collection, missingElements))
                {
                    usersToCollaborate.Add(user.Username);
                }
            }

            return Ok(usersToCollaborate);
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

        private async Task<SetData?> GetSetDataByName(string setName)
        {
            var setSummary = await apiClient.Api.Set.ByName[setName].GetAsync();
            if (setSummary?.Id is null)
                return null;

            return await GetSetDataById(setSummary.Id);
        }

        private async Task<SetData?> GetSetDataById(string id)
        {
            var setDataById = await apiClient.Api.Set.ById[id].GetAsync();
            return setDataById?.ToSetData();
        }

        private bool CanBuildSet(Dictionary<(string pieceId, string color), int> userCollection, Dictionary<(string pieceId, string color), int> setPieces)
            => !setPieces.Any(p => !userCollection.ContainsKey(p.Key) || userCollection[p.Key] < p.Value);
    }
}
