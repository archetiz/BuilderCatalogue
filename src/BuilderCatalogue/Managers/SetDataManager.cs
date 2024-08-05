using BrickApi.Client;
using BrickApi.Client.Api.Sets;
using BuilderCatalogue.DTOs;
using BuilderCatalogue.Extensions;

namespace BuilderCatalogue.Managers
{
    public class SetDataManager(BrickApiClient apiClient) : ISetDataManager
    {
        public async Task<IEnumerable<SetsGetResponse_Sets>> GetAllSets()
        {
            var setsResponse = await apiClient.Api.Sets.GetAsync();
            return setsResponse?.Sets ?? Enumerable.Empty<SetsGetResponse_Sets>();
        }

        public async Task<SetData?> GetSetDataByName(string setName)
        {
            var setSummary = await apiClient.Api.Set.ByName[setName].GetAsync();
            if (setSummary?.Id is null)
                return null;

            return await GetSetDataById(setSummary.Id);
        }

        public virtual async Task<SetData?> GetSetDataById(string id)
        {
            var setDataById = await apiClient.Api.Set.ById[id].GetAsync();
            return setDataById?.ToSetData();
        }
    }
}
