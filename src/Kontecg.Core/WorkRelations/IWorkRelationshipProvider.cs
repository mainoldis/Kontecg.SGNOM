using System.Threading.Tasks;

namespace Kontecg.WorkRelations
{
    /// <summary>
    ///     Provides an interface to provide last employment info in the upper layers.
    /// </summary>
    public interface IWorkRelationshipProvider
    {
        WorkRelationship[] GetWorkRelationshipInformation();

        Task<WorkRelationship[]> GetWorkRelationshipInformationAsync();
    }
}
