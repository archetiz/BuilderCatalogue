using BrickApi.Client.Api.Users;
using BuilderCatalogue.DTOs;

namespace BuilderCatalogue.Managers
{
    public interface IUserDataManager
    {
        Task<IEnumerable<UsersGetResponse_Users>> GetAllUsers();
        Task<UserData?> GetUserDataById(string id);
        Task<UserData?> GetUserDataByName(string username);
    }
}