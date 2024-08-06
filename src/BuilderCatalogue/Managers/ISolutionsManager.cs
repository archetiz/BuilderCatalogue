
namespace BuilderCatalogue.Managers
{
    public interface ISolutionsManager
    {
        Task<IEnumerable<string>> SolveFirstAssignment(string username = "brickfan35");
        Task<IEnumerable<string>> SolveSecondAssignment(string username = "landscape-artist", string setName = "tropical-island");
        int SolveThirdAssignment();
        Task<IEnumerable<string>> SolveFourthAssignment(string username = "dr_crocodile");
    }
}
