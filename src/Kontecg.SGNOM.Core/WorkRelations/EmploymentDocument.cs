using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Itenso.TimePeriod;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.Extensions;
using Kontecg.HumanResources;
using Kontecg.Identity;
using Kontecg.MultiCompany;
using Kontecg.Organizations;
using Kontecg.Salary;
using Kontecg.Text;
using Kontecg.Timing;
using Kontecg.Workflows;
using NMoneys;
using static Kontecg.Text.FormattedStringValueExtracter;

namespace Kontecg.WorkRelations
{
    [Table("employment_documents", Schema = "rel")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class EmploymentDocument : AuditedEntity<long>, IMustHaveCompany, IMustHaveOrganizationUnit, IMustHaveReview
    {
        public const int MaxExtraSummaryLength = 500;

        public const int MaxCodeLength = 11;

        [Required]
        public virtual int DocumentDefinitionId { get; set; }

        [NotMapped]
        public virtual DocumentDefinition DocumentDefinition { get; set; }

        [Required]
        public virtual DateTime MadeOn { get; set; }

        [Required]
        public virtual DateTime EffectiveSince { get; set; }

        [NotMapped]
        public DateTime EffectiveUntil
        {
            get
            {
                if (Type == EmploymentType.B)
                    return EffectiveSince;

                if (Children != null && Children.Any(c => c.Review == ReviewStatus.Confirmed))
                    return Children.OrderByDescending(c => c.EffectiveSince)
                        .First(c => c.Review == ReviewStatus.Confirmed).EffectiveSince;

                return ExpirationDate ?? DateTime.MaxValue;
            }
        }

        [Required]
        [Range(1, Int32.MaxValue)]
        public virtual int Exp { get; set; }

        [Required]
        public virtual long PersonId { get; set; }

        [NotMapped]
        public virtual Person Person { get; set; }

        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        [Required]
        public virtual Guid GroupId { get; set; }

        [Required]
        public virtual EmployeeSalaryForm EmployeeSalaryForm { get; set; }

        [Required]
        public virtual int CompanyId { get; set; }

        [NotMapped]
        public virtual Company Company { get; set; }

        [Required]
        public virtual ContractType Contract { get; set; }

        public virtual ContractSubType? ContractSubType { get; set; }

        [Required]
        public virtual EmploymentType Type { get; set; }

        [Required]
        public virtual EmploymentSubType SubType { get; set; }

        public virtual long? PreviousId { get; set; }

        [ForeignKey("PreviousId")]
        public virtual EmploymentDocument Previous { get; set; }

        [Required]
        public virtual long OrganizationUnitId { get; set; }

        [NotMapped]
        public virtual WorkPlaceUnit WorkPlaceUnit { get; set; }

        [Required]
        [StringLength(WorkPlacePayment.MaxCodeLength)]
        public virtual string WorkPlacePaymentCode { get; set; }

        [Required]
        [StringLength(OrganizationUnit.MaxCodeLength)]
        public virtual string FirstLevelCode { get; set; }

        [Required]
        [StringLength(OrganizationUnit.MaxDisplayNameLength)]
        public virtual string FirstLevelDisplayName { get; set; }

        [Required]
        [StringLength(OrganizationUnit.MaxCodeLength)]
        public virtual string SecondLevelCode { get; set; }

        [Required]
        [StringLength(OrganizationUnit.MaxDisplayNameLength)]
        public virtual string SecondLevelDisplayName { get; set; }

        [Required]
        [StringLength(OrganizationUnit.MaxCodeLength)]
        public virtual string ThirdLevelCode { get; set; }

        [Required]
        [StringLength(OrganizationUnit.MaxDisplayNameLength)]
        public virtual string ThirdLevelDisplayName { get; set; }

        [Required]
        public virtual int CenterCost { get; set; }

        [Required]
        [StringLength(Organizations.ComplexityGroup.MaxGroupLength)]
        public virtual string ComplexityGroup { get; set; }

        [Required]
        [StringLength(Organizations.Occupation.MaxDisplayNameLength)]
        public virtual string Occupation { get; set; }

        [Required]
        [StringLength(Organizations.Occupation.MaxCodeMaxLength)]
        public virtual string OccupationCode { get; set; }

        [StringLength(Organizations.Responsibility.MaxDisplayNameLength)]
        public virtual string Responsibility { get; set; }

        [NotMapped]
        public virtual string FullOccupationDescription => $"{Occupation}{(!Responsibility.IsNullOrWhiteSpace() && Responsibility.ToUpperInvariant() != "DEL CARGO" ? " (" + Responsibility + ")" : "")}";

        [Required]
        public virtual char OccupationCategory { get; set; }

        public virtual int WorkShiftId { get; set; }

        [Required]
        [ForeignKey("WorkShiftId")]
        public virtual WorkShift WorkShift { get; set; }

        [Required]
        public virtual Money Salary { get; set; }

        [Required]
        public virtual Money TotalSalary { get; set; }

        [Required]
        public virtual decimal RatePerHour { get; set; }

        public virtual int SummaryId { get; set; }

        [Required]
        [ForeignKey("SummaryId")]
        public virtual EmploymentSummary Summary { get; set;}

        [StringLength(MaxExtraSummaryLength)]
        public virtual string ExtraSummary { get; set; }

        [NotMapped]
        public virtual string DisplaySummary => $"{Summary}({Contract}) {ExtraSummary}";

        [Range(1, Int32.MaxValue)]
        public virtual int? LegalNumber { get; set; }

        public virtual DateTime? ExpirationDate { get; set; }

        [Required]
        public virtual bool HasSalary { get; set; }

        [Required]
        public virtual bool ByAssignment { get; set; }

        [Required]
        public virtual bool ByOfficial { get; set; }

        public virtual List<EmploymentPlus> Plus { get; set; }

        public virtual List<EmploymentDocument> Children { get; set; }

        public virtual int? MadeById { get; set; }

        [ForeignKey("MadeById")]
        public virtual SignOnDocument MadeBy { get; set; }

        public virtual int? ReviewedById { get; set; }

        [ForeignKey("ReviewedById")]
        public virtual SignOnDocument ReviewedBy { get; set; }

        public virtual int? ApprovedById { get; set; }

        [ForeignKey("ApprovedById")]
        public virtual SignOnDocument ApprovedBy { get; set; }

        [Required]
        public virtual ReviewStatus Review { get; set; }

        [NotMapped]
        public virtual ITimePeriod Period => new TimeRange(EffectiveSince, EffectiveUntil);

        public EmploymentDocument()
        {
            HasSalary = true;
            ByAssignment = false;
            ByOfficial = false;
            Review = ReviewStatus.ForReview;
            Plus = [];
            Children = [];
        }

        public void SetGroup(Guid guid)
        {
            switch (Type)
            {
                case EmploymentType.A:
                    GroupId = Previous is {Type: EmploymentType.B} ? Previous.GroupId : guid;
                    break;
                default:
                {
                    GroupId = Previous?.GroupId ?? guid;
                    break;
                }
            }
        }

        public void CalculateSalary()
        {
            var plus = Plus.Select(p => p.Amount).ToArray();
            TotalSalary = HasSalary
                ? Money.Total(Salary, plus.Length > 0 ? Money.Total(plus) : Money.Zero(KontecgCoreConsts.DefaultCurrency))
                : Money.Zero(KontecgCoreConsts.DefaultCurrency);
        }

        public void SetRatePerHour(decimal? averageWorkingHoursPerPeriod = null)
        {
            RatePerHour = decimal.Divide(Salary.Amount, averageWorkingHoursPerPeriod > 0 && averageWorkingHoursPerPeriod.Value != SGNOMConsts.DefaultAverageWorkingHoursPerPeriod
                ? averageWorkingHoursPerPeriod.Value
                : SGNOMConsts.DefaultAverageWorkingHoursPerPeriod);
        }

        public void SetExpirationDate(DateTime expirationDate)
        {
            if (Contract == ContractType.I &&
                (SubType == EmploymentSubType.I || SubType == EmploymentSubType.DI))
            {
                ExpirationDate = null;
                return;
            }

            ExpirationDate = expirationDate;
        }

        public WorkYear GetSchedule(DateTime? since = null)
        {
            return WorkShift is {Regime: not null} ? new WorkYear(since ?? EffectiveSince, WorkShift.ToWorkPattern()) : null;
        }

        public WorkYear GetSchedule(ITimeCalendar calendar, DateTime? since = null)
        {
            return WorkShift is { Regime: not null} ? new WorkYear(since ?? EffectiveSince, WorkShift.ToWorkPattern(calendar)) : null;
        }

        private ExtractionResult IsOccupationValid(string[] occupationFormats, string occupation)
        {
            foreach (var item in occupationFormats)
            {
                var occupationFormat = item.Replace("(", "").Replace(")", "");
                var result = new FormattedStringValueExtracter().Extract(occupation, occupationFormat, true, ' ');

                if (result.IsMatch && result.Matches.Any())
                {
                    return result;
                }
            }
            return null;
        }

        public EmploymentDocument CreateACopy()
        {
            var newDoc = new EmploymentDocument();
            newDoc.DocumentDefinitionId = this.DocumentDefinitionId;
            newDoc.MadeOn = Clock.Now;
            newDoc.EffectiveSince = newDoc.MadeOn;
            newDoc.Exp = this.Exp;
            newDoc.PersonId = this.PersonId;
            newDoc.GroupId = this.GroupId;
            newDoc.Contract = this.Contract;
            newDoc.ContractSubType = this.ContractSubType;
            newDoc.Type = this.Type;
            newDoc.SubType = this.SubType;
            newDoc.EmployeeSalaryForm = this.EmployeeSalaryForm;
            newDoc.PreviousId = this.Id;
            newDoc.OrganizationUnitId = this.OrganizationUnitId;
            newDoc.WorkPlacePaymentCode = this.WorkPlacePaymentCode;
            newDoc.FirstLevelCode = this.FirstLevelCode;
            newDoc.FirstLevelDisplayName = this.FirstLevelDisplayName;
            newDoc.SecondLevelCode = this.SecondLevelCode;
            newDoc.SecondLevelDisplayName = this.SecondLevelDisplayName;
            newDoc.ThirdLevelCode = this.ThirdLevelCode;
            newDoc.ThirdLevelDisplayName = this.ThirdLevelDisplayName;
            newDoc.CenterCost = this.CenterCost;
            newDoc.ComplexityGroup = this.ComplexityGroup;
            newDoc.Occupation = this.Occupation;
            newDoc.OccupationCode = this.OccupationCode;
            newDoc.Responsibility = this.Responsibility;
            newDoc.OccupationCategory = this.OccupationCategory;
            newDoc.WorkShiftId = this.WorkShiftId;
            newDoc.SummaryId = this.SummaryId;
            newDoc.ExtraSummary = this.ExtraSummary;
            newDoc.ExpirationDate = this.ExpirationDate;
            newDoc.HasSalary = this.HasSalary;
            newDoc.ByOfficial = this.ByOfficial;
            newDoc.ByAssignment = this.ByAssignment;
            newDoc.Plus.AddRange(Plus);
            newDoc.Salary = this.Salary;
            newDoc.TotalSalary = this.TotalSalary;
            return newDoc;
        }

        public override string ToString()
        {
            return $"{base.ToString()}, {nameof(Exp)}: {Exp}";
        }
    }
}
