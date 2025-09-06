using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Itenso.TimePeriod;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Extensions;
using Kontecg.Organizations;
using Kontecg.Timing;
using NMoneys;

namespace Kontecg.WorkRelations
{
    public class EmploymentDocumentGenerator : SGNOMServiceBase, IEmploymentDocumentGenerator, IShouldInitialize, ISingletonDependency
    {
        private readonly IOccupationRepository _occupationRepository;
        private readonly IRepository<WorkPlaceUnit, long> _workPlaceUnitRepository;
        private readonly IWorkShiftRepository _workShiftRepository;
        private readonly ITemplateRepository _templateRepository;
        private readonly ITemplateJobPositionRepository _templateJobPositionRepository;
        private readonly IRepository<EmploymentSummary> _employmentSummaryRepository;

        private List<Occupation> _occupations;
        private List<WorkPlaceUnit> _workPlaceUnits;
        private List<EmploymentSummary> _summaries;
        private List<WorkShift> _workShifts;

        /// <inheritdoc />
        public EmploymentDocumentGenerator(
            IOccupationRepository occupationRepository, 
            IRepository<WorkPlaceUnit, long> workPlaceUnitRepository, 
            ITemplateRepository templateRepository, 
            ITemplateJobPositionRepository templateJobPositionRepository, 
            IRepository<EmploymentSummary> employmentSummaryRepository, 
            IWorkShiftRepository workShiftRepository)
        {
            _occupationRepository = occupationRepository;
            _workPlaceUnitRepository = workPlaceUnitRepository;
            _templateRepository = templateRepository;
            _templateJobPositionRepository = templateJobPositionRepository;
            _employmentSummaryRepository = employmentSummaryRepository;
            _workShiftRepository = workShiftRepository;

            LocalizationSourceName = SGNOMConsts.LocalizationSourceName;
        }

        /// <inheritdoc />
        public void Initialize()
        {
            _occupations = _occupationRepository.GetAllIncluding(o => o.Category,
                                                    o => o.Group,
                                                    o => o.Responsibility)
                                                .Where(o => o.IsActive).ToList();

            _workPlaceUnits = _workPlaceUnitRepository.GetAllIncluding(c => c.WorkPlacePayment).ToList();
            _summaries = _employmentSummaryRepository.GetAllIncluding(s => s.Parent).ToList();
            _workShifts = _workShiftRepository.GetAllIncluding(w => w.Regime).Where(w => w.IsActive).ToList();
        }

        /// <inheritdoc />
        public EmploymentDocument Clone(EmploymentDocument document)
        {
            var newDocument = document.CreateACopy();
            newDocument.PreviousId = document.Id;
            newDocument.Type = EmploymentType.R;

            newDocument.Plus.Clear();
            for (int i = 0; i < document.Plus.Count; i++)
            {
                var plus = new EmploymentPlus
                {
                    PlusDefinition = document.Plus[i].PlusDefinition
                };
                plus.SetAmount(document.Plus[i].Amount);
                plus.SetRatePerHour();
                plus.Employment = newDocument;
                newDocument.Plus.Add(plus);
            }

            newDocument.CalculateSalary();
            newDocument.SetRatePerHour();

            newDocument.LegalNumber = null;
            newDocument.MadeBy = null;
            newDocument.ReviewedBy = null;
            newDocument.ApprovedBy = null;

            var wp = _workPlaceUnits.FirstOrDefault(w => w.Id == document.OrganizationUnitId);
            if (wp != null)
            {
                newDocument.OrganizationUnitId = wp.Id;
                var ancestors = _templateRepository.GetOrganizationUnitAncestor(wp.Id);
                newDocument.FirstLevelCode = ancestors.FirstOrDefault(a => a.Level == 1)?.Code ?? "";
                newDocument.FirstLevelDisplayName = ancestors.FirstOrDefault(a => a.Level == 1)?.DisplayName ?? "";
                newDocument.SecondLevelCode = ancestors.FirstOrDefault(a => a.Level == 2)?.Code ?? "";
                newDocument.SecondLevelDisplayName = ancestors.FirstOrDefault(a => a.Level == 2)?.DisplayName ?? "";
                newDocument.ThirdLevelCode = wp.Code;
                newDocument.ThirdLevelDisplayName = wp.DisplayName;
                newDocument.WorkPlacePaymentCode = wp.WorkPlacePayment?.Code ?? "";
            }

            newDocument.SummaryId = _summaries.Find(s => s.Reference == "R_OTROS_GENERICO").Id;
            return newDocument;
        }

        /// <inheritdoc />
        public EmploymentDocument Clone(EmploymentDocument document, EmploymentDocumentToGenerate legalChange)
        {
            var newDocument = document.CreateACopy();

            if (legalChange.EffectiveSince != null)
            {
                newDocument.MadeOn = legalChange.EffectiveSince.Value;
                newDocument.EffectiveSince = legalChange.EffectiveSince.Value;
            }

            newDocument.PreviousId = document.Id;
            newDocument.Type = EmploymentType.R;

            newDocument.Plus.Clear();
            for (int i = 0; i < document.Plus.Count; i++)
            {
                var plus = new EmploymentPlus
                {
                    PlusDefinition = document.Plus[i].PlusDefinition
                };
                plus.SetAmount(document.Plus[i].Amount);
                plus.SetRatePerHour();
                plus.Employment = newDocument;
                newDocument.Plus.Add(plus);
            }

            newDocument.CalculateSalary();
            newDocument.SetRatePerHour();

            newDocument.LegalNumber = null;
            newDocument.MadeBy = null;
            newDocument.ReviewedBy = null;
            newDocument.ApprovedBy = null;

            var changeType = legalChange.LegalChangeType;
            
            if ((changeType & EmploymentDocumentLegalChangeType.Validity) == EmploymentDocumentLegalChangeType.Validity)
            {
                if (document.SubType == EmploymentSubType.MP | document.SubType == EmploymentSubType.PMP)
                {
                    newDocument.SubType = EmploymentSubType.PMP;
                    var workShift = _workShifts.First(w => w.Id == newDocument.WorkShiftId);
                    var pattern = new WorkYear(newDocument.EffectiveSince, workShift.ToWorkPattern());
                    var expirationDate = legalChange.ExpirationDate ?? newDocument.EffectiveSince.AddDays(90);
                    if (pattern.WorkingPeriods.HasIntersectionPeriods(expirationDate))
                    {
                        var periods = pattern.WorkingPeriods.IntersectionPeriods(expirationDate);
                        periods.SortByEnd(ListSortDirection.Descending);
                        newDocument.SetExpirationDate(periods.End);
                    }
                }
            }

            if ((changeType & EmploymentDocumentLegalChangeType.Type) == EmploymentDocumentLegalChangeType.Type)
            {
                if (legalChange.Type != null && document.Type != legalChange.Type) newDocument.Type = legalChange.Type.Value;
                if (legalChange.SubType != null && document.SubType != legalChange.SubType) newDocument.SubType = legalChange.SubType.Value;
            }

            if ((changeType & EmploymentDocumentLegalChangeType.OrganizationUnit) == EmploymentDocumentLegalChangeType.OrganizationUnit)
            {
                if (legalChange.OrganizationUnitId != null)
                {
                    var wp = _workPlaceUnits.FirstOrDefault(w => w.Id == legalChange.OrganizationUnitId);
                    if (wp != null)
                    {
                        newDocument.OrganizationUnitId = wp.Id;
                        var ancestors = _templateRepository.GetOrganizationUnitAncestor(wp.Id);
                        newDocument.FirstLevelCode = ancestors.FirstOrDefault(a => a.Level == 1)?.Code ?? "";
                        newDocument.FirstLevelDisplayName = ancestors.FirstOrDefault(a => a.Level == 1)?.DisplayName ?? "";
                        newDocument.SecondLevelCode = ancestors.FirstOrDefault(a => a.Level == 2)?.Code ?? "";
                        newDocument.SecondLevelDisplayName = ancestors.FirstOrDefault(a => a.Level == 2)?.DisplayName ?? "";
                        newDocument.ThirdLevelCode = wp.Code;
                        newDocument.ThirdLevelDisplayName = wp.DisplayName;
                        newDocument.WorkPlacePaymentCode = wp.WorkPlacePayment?.Code ?? "";
                    }
                    else
                    {
                        newDocument.OrganizationUnitId = legalChange.OrganizationUnitId.Value;
                        newDocument.FirstLevelCode = "";
                        newDocument.FirstLevelDisplayName = "";
                        newDocument.SecondLevelCode = "";
                        newDocument.SecondLevelDisplayName = "";
                        newDocument.ThirdLevelCode = "";
                        newDocument.ThirdLevelDisplayName = "";
                        newDocument.WorkPlacePaymentCode = "";
                    }
                }
            }

            if ((changeType & EmploymentDocumentLegalChangeType.CenterCost) == EmploymentDocumentLegalChangeType.CenterCost)
            {
                if (legalChange.CenterCost != null && document.CenterCost != legalChange.CenterCost) newDocument.CenterCost = legalChange.CenterCost.Value;
            }

            if ((changeType & EmploymentDocumentLegalChangeType.Occupation) == EmploymentDocumentLegalChangeType.Occupation)
            {
                if (!legalChange.OccupationCode.IsNullOrWhiteSpace() && document.OccupationCode != legalChange.OccupationCode)
                {
                    var newOccupation = _occupations.FirstOrDefault(o => o.Code == legalChange.OccupationCode);
                    if (newOccupation != null)
                    {
                        newDocument.OccupationCode = newOccupation.Code;
                        newDocument.Occupation = newOccupation.DisplayName;
                        newDocument.Responsibility = newOccupation.Responsibility.DisplayName;
                        newDocument.OccupationCategory = newOccupation.Category.Code;
                        newDocument.ComplexityGroup = newOccupation.Group.Group;
                        newDocument.Salary = newOccupation.Group.BaseSalary;
                    }
                    else
                    {
                        newDocument.OccupationCode = legalChange.OccupationCode;
                        newDocument.Occupation = "";
                        newDocument.Responsibility = "";
                        newDocument.OccupationCategory = '\0';
                        newDocument.ComplexityGroup = "";
                        newDocument.Salary = Money.Zero(document.Salary.CurrencyCode);
                    }
                    
                    newDocument.CalculateSalary();
                    newDocument.SetRatePerHour();
                }
            }

            if ((changeType & EmploymentDocumentLegalChangeType.Plus) == EmploymentDocumentLegalChangeType.Plus)
            {
                newDocument.Plus.Clear();
                newDocument.CalculateSalary();
            }

            if ((changeType & EmploymentDocumentLegalChangeType.WorkShift) == EmploymentDocumentLegalChangeType.WorkShift)
            {
                if (legalChange.WorkShiftId != null && document.WorkShiftId != legalChange.WorkShiftId)
                {
                    newDocument.WorkShiftId = legalChange.WorkShiftId.Value;
                    var workShift = _workShifts.First(w => w.Id == newDocument.WorkShiftId);
                    var pattern = new WorkYear(newDocument.EffectiveSince, workShift.ToWorkPattern());
                    if (pattern.WorkingPeriods.HasIntersectionPeriods(newDocument.EffectiveSince))
                    {
                        var periods = pattern.WorkingPeriods.IntersectionPeriods(newDocument.EffectiveSince);
                        periods.SortByEnd();
                        newDocument.EffectiveSince = periods.End;
                    }

                    if (newDocument.ExpirationDate != null)
                    {
                        pattern = new WorkYear(newDocument.ExpirationDate.Value, workShift.ToWorkPattern());
                        if(pattern.WorkingPeriods.HasIntersectionPeriods(newDocument.ExpirationDate.Value))
                        {
                            var periods = pattern.WorkingPeriods.IntersectionPeriods(newDocument.ExpirationDate.Value);
                            periods.SortByEnd(ListSortDirection.Descending);
                            newDocument.SetExpirationDate(periods.End);
                        }
                    }
                }
            }

            if ((changeType & EmploymentDocumentLegalChangeType.EmployeeSalaryForm) == EmploymentDocumentLegalChangeType.EmployeeSalaryForm)
            {
                if (legalChange.EmployeeSalaryForm != null &&
                    document.EmployeeSalaryForm != legalChange.EmployeeSalaryForm)
                {
                    newDocument.EmployeeSalaryForm = legalChange.EmployeeSalaryForm.Value;
                }
            }

            if (legalChange.SummaryId != null)
            {
                newDocument.SummaryId = legalChange.SummaryId.Value;
            }

            if (!legalChange.ExtraSummary.IsNullOrWhiteSpace())
            {
                newDocument.ExtraSummary = legalChange.ExtraSummary;
            }

            return newDocument;
        }
    }
}