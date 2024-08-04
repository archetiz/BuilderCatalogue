using BrickApi.Client.Api.Sets;
using BuilderCatalogue.DTOs;

namespace BuilderCatalogue.Managers
{
    public interface ISetDataManager
    {
        Task<IEnumerable<SetsGetResponse_Sets>> GetAllSets();
        Task<SetData?> GetSetDataById(string id);
        Task<SetData?> GetSetDataByName(string setName);
    }
}