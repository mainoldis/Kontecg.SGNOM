using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;
using Kontecg.Domain.Entities.Auditing;
using Kontecg.MultiCompany;
using Kontecg.Timing;
using Kontecg.Workflows;
using Kontecg.WorkRelations;
using NMoneys;

namespace Kontecg.Holidays
{
    /// <summary>  
    /// Represents a holiday document entity.  
    /// </summary>  
    [Table("holiday_documents", Schema = "vac")]
    [MultiCompanySide(MultiCompanySides.Company)]
    public class HolidayDocument : CreationAuditedEntity<long>, IMustHaveCompany, IMustHaveReview
    {
        /// <summary>  
        /// Maximum length for the document code.  
        /// </summary>  
        public const int MaxCodeLength = 20;

        /// <summary>  
        /// Gets or sets the document definition ID.  
        /// </summary>  
        [Required]
        public virtual int DocumentDefinitionId { get; set; }

        /// <summary>  
        /// Gets or sets the date the document was created.  
        /// </summary>  
        [Required]
        public DateTime MadeOn { get; set; }

        /// <summary>  
        /// Gets or sets the unique code of the document.  
        /// </summary>  
        [Required]
        [StringLength(MaxCodeLength)]
        public virtual string Code { get; set; }

        /// <summary>  
        /// Gets or sets the ID of the person associated with the holiday.  
        /// </summary>  
        [Required]
        public virtual long PersonId { get; set; }

        /// <summary>  
        /// Gets or sets the start date of the holiday.  
        /// </summary>  
        [Required]
        public virtual DateTime Since { get; set; }

        /// <summary>  
        /// Gets or sets the end date of the holiday.  
        /// </summary>  
        [Required]
        public virtual DateTime Until { get; set; }

        /// <summary>  
        /// Gets or sets the return date after the holiday.  
        /// </summary>  
        [Required]
        public virtual DateTime Return { get; set; }

        /// <summary>  
        /// Gets or sets the type of the holiday.  
        /// </summary>  
        [Required]
        public virtual HolidayType Type { get; set; }

        /// <summary>  
        /// Gets or sets the employment ID associated with the holiday.  
        /// </summary>  
        public virtual long EmploymentId { get; set; }

        /// <summary>  
        /// Gets or sets the employment document associated with the holiday.  
        /// </summary>  
        [Required]
        [ForeignKey("EmploymentId")]
        public virtual EmploymentDocument EmploymentDocument { get; set; }

        /// <summary>  
        /// Gets or sets the group ID associated with the holiday.  
        /// </summary>  
        [Required]
        public virtual Guid GroupId { get; set; }

        /// <summary>  
        /// Gets or sets the number of days for the holiday.  
        /// </summary>  
        [Required]
        public virtual int Days { get; set; }

        /// <summary>  
        /// Gets or sets the number of hours for the holiday.  
        /// </summary>  
        [Required]
        public virtual decimal Hours { get; set; }

        /// <summary>  
        /// Gets or sets the monetary amount associated with the holiday.  
        /// </summary>  
        [Required]
        public virtual Money Amount { get; set; }

        /// <summary>  
        /// Gets or sets the currency for the holiday. Default is CUP.  
        /// </summary>  
        [Required]
        public virtual CurrencyIsoCode Currency { get; set; }

        /// <summary>  
        /// Gets or sets the rate per day for the holiday.  
        /// </summary>  
        [Required]
        public virtual decimal RatePerDay { get; set; }

        /// <summary>  
        /// Gets or sets the company ID associated with the holiday.  
        /// </summary>  
        [Required]
        public virtual int CompanyId { get; set; }

        /// <summary>  
        /// Gets or sets the review status of the holiday document.  
        /// </summary>  
        [Required]
        public virtual ReviewStatus Review { get; set; }

        /// <summary>  
        /// Gets or sets the list of notes associated with the holiday.  
        /// </summary>  
        public virtual List<HolidayNote> Notes { get; set; }

        /// <summary>  
        /// Initializes a new instance of the <see cref="HolidayDocument"/> class with default values.  
        /// </summary>  
        public HolidayDocument()
        {
            Currency = CurrencyIsoCode.CUP;
            Review = ReviewStatus.ForReview;
            Notes = new List<HolidayNote>();
        }

        /// <summary>  
        /// Initializes a new instance of the <see cref="HolidayDocument"/> class with specified values.  
        /// </summary>  
        /// <param name="documentDefinitionId">The document definition ID.</param>  
        /// <param name="personId">The person ID associated with the holiday.</param>  
        /// <param name="employmentId">The employment ID associated with the holiday.</param>  
        /// <param name="groupId">The group ID associated with the holiday.</param>  
        /// <param name="type">The type of the holiday. Default is Anticipated.</param>  
        public HolidayDocument(int documentDefinitionId, long personId, long employmentId, Guid groupId, HolidayType type = HolidayType.Anticipated)
            : this()
        {
            DocumentDefinitionId = documentDefinitionId;
            PersonId = personId;
            EmploymentId = employmentId;
            GroupId = groupId;
            Type = type;

            NormalizeDates();
        }

        /// <summary>  
        /// Normalizes the dates for the holiday document based on its type.  
        /// </summary>  
        private void NormalizeDates()
        {
            if (Type != HolidayType.Liquidation)
                return;

            MadeOn = Clock.Now;
            Since = Clock.Now;
            Until = Since;
            Return = Since;
        }
    }
}
