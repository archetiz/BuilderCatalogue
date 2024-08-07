using BrickApi.Client.Api.Users;
using BuilderCatalogue.DTOs;

namespace BuilderCatalogue.Managers
{
    public interface IUserDataManager
    {
        Task<IEnumerable<UsersGetResponse_Users>> GetAllUsers();
        Task<IEnumerable<UserData>> GetAllUsersDataWithDetails(params string[] except);
        Task<UserData?> GetUserDataById(string id);
        Task<UserData?> GetUserDataByName(string username);
    }
}
