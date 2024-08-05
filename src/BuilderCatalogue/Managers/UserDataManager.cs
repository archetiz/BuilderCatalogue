using BrickApi.Client;
using BrickApi.Client.Api.Users;
using BuilderCatalogue.DTOs;
using BuilderCatalogue.Extensions;

namespace BuilderCatalogue.Managers
{
    public class UserDataManager(BrickApiClient apiClient) : IUserDataManager
    {
        public async Task<IEnumerable<UsersGetResponse_Users>> GetAllUsers()
        {
            var usersResponse = await apiClient.Api.Users.GetAsync();
            return usersResponse?.Users ?? Enumerable.Empty<UsersGetResponse_Users>();
        }

        public async Task<UserData?> GetUserDataByName(string username)
        {
            var userSummary = await apiClient.Api.User.ByUsername[username].GetAsync();
            if (userSummary?.Id is null)
                return null;

            return await GetUserDataById(userSummary.Id);
        }

        public virtual async Task<UserData?> GetUserDataById(string id)
        {
            var userDataById = await apiClient.Api.User.ById[id].GetAsync();
            return userDataById?.ToUserData();
        }
    }
}
