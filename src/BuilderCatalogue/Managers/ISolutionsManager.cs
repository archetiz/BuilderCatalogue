
namespace BuilderCatalogue.Managers
{
    public interface ISolutionsManager
    {
        Task<IEnumerable<string>> SolveFirstAssignment();
        Task<IEnumerable<string>> SolveFourthAssignment();
        Task<IEnumerable<string>> SolveSecondAssignment();
        Task<int> SolveThirdAssignment();
    }
}