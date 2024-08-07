
namespace BuilderCatalogue.Managers
{
    public interface ISolutionsManager
    {
        Task<IEnumerable<string>> SolveFirstAssignment(string username = "brickfan35");
        Task<IEnumerable<string>> SolveSecondAssignment(string username = "landscape-artist", string setName = "tropical-island");
        Task<int> SolveThirdAssignment(string username = "megabuilder99");
        Task<IEnumerable<string>> SolveFourthAssignment(string username = "dr_crocodile");
    }
}
