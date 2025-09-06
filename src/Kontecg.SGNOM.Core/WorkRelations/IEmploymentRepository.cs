using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Kontecg.Domain.Repositories;
using Kontecg.Workflows;

namespace Kontecg.WorkRelations
{
    public interface IEmploymentRepository : IRepository<EmploymentDocument, long>
    {
        EmploymentDocument GetRecentAncestorForProvisional(long employmentId);

        Task<EmploymentDocument> GetRecentAncestorForProvisionalAsync(long employmentId);

        /// <summary>
        ///    Search for the last payroll movements of the person grouped by their employment
        /// </summary>
        /// <param name="personId">person id</param>
        /// <returns></returns>
        IReadOnlyList<EmploymentDocument> LastByPersonId(long personId);

        /// <summary>
        ///    Search for the last payroll movements of the person grouped by their employment
        /// </summary>
        /// <param name="personId">person id</param>
        /// <returns></returns>
        Task<IReadOnlyList<EmploymentDocument>> LastByPersonIdAsync(long personId);

        /// <summary>
        ///    Search for the last payroll movements of the person grouped by their employment
        /// </summary>
        /// <param name="exp">Exp</param>
        /// <returns></returns>
        IReadOnlyList<EmploymentDocument> LastByExp(int exp);

        /// <summary>
        ///    Search for the last payroll movements of the person grouped by their employment
        /// </summary>
        /// <param name="exp">Exp</param>
        /// <returns></returns>
        Task<IReadOnlyList<EmploymentDocument>> LastByExpAsync(int exp);

        IReadOnlyList<EmploymentDocument> RelationshipByPeriod(IFilterRequest inputFilterRequest);

        Task<IReadOnlyList<EmploymentDocument>> RelationshipByPeriodAsync(IFilterRequest inputFilterRequest);

        /// <summary>
        ///    Search for the last payroll movements of the person grouped by their employment, includes unemployed persons
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<EmploymentDocument> CurrentRelationship(IFilterRequest inputFilterRequest = null);

        /// <summary>
        ///    Search for the last payroll movements of the person grouped by their employment, includes unemployed persons
        /// </summary>
        /// <returns></returns>
        Task<IReadOnlyList<EmploymentDocument>> CurrentRelationshipAsync(IFilterRequest inputFilterRequest = null);

        IReadOnlyList<EmploymentDocument> GetDocumentToExpireSoon();

        Task<IReadOnlyList<EmploymentDocument>> GetDocumentToExpireSoonAsync();

        IReadOnlyList<EmploymentDocument> GetDocumentsToReview();

        Task<IReadOnlyList<EmploymentDocument>> GetDocumentsToReviewAsync();

        IReadOnlyList<EmploymentDocument> GetDocumentsReviewed();

        Task<IReadOnlyList<EmploymentDocument>> GetDocumentsReviewedAsync();

        IQueryable<EmploymentDocument> GetQueryableEmploymentDocument();

        Task<IQueryable<EmploymentDocument>> GetQueryableEmploymentDocumentAsync();

        void UpdateEmploymentsCenterCost(int centerCost, [CanBeNull] params long[] employmentIds);

        Task UpdateEmploymentsCenterCostAsync(int centerCost, [CanBeNull] params long[] employmentIds);

        void UpdateEmploymentsWorkPlace(long workPlaceUnitId, [CanBeNull] params long[] employmentIds);

        Task UpdateEmploymentsWorkPlaceAsync(long workPlaceUnitId, [CanBeNull] params long[] employmentIds);

        void UpdateReviewStatus(ReviewStatus status, [CanBeNull] params long[] employmentIds);

        Task UpdateReviewStatusAsync(ReviewStatus status, [CanBeNull] params long[] employmentIds);
    }
}
