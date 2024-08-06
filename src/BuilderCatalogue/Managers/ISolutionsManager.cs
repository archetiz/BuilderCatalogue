
namespace BuilderCatalogue.Managers
{
    public interface ISolutionsManager
    {
        Task<IEnumerable<string>> SolveFirstAssignment(string user = "brickfan35");
        Task<IEnumerable<string>> SolveFourthAssignment();
        Task<IEnumerable<string>> SolveSecondAssignment();
        Task<int> SolveThirdAssignment();
    }
}
