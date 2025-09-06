using System;
using System.ComponentModel.DataAnnotations;
using Itenso.TimePeriod;
using Kontecg.Application.Services.Dto;
using Kontecg.Extensions;
using Kontecg.Runtime.Validation;
using static Kontecg.Timing.DayDecorator;

namespace Kontecg.Timing.Dto
{
    public class SpecialDateDto : EntityDto, IShouldNormalize, ICustomValidate
    {
        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Cause { get; set; }

        public void Normalize()
        {
            Date = TimeTrim.Hour(Date);
        }

        public void AddValidationErrors(CustomValidationContext context)
        {
            if (!Cause.IsNullOrEmpty() && string.Equals(Cause, BreakSaturday.ToString(), StringComparison.CurrentCultureIgnoreCase) &&
                Date.DayOfWeek != DayOfWeek.Saturday)
            {
                context.Results.Add(new ValidationResult("Date must be a Saturday if Cause is specified in same way",
                    new[] {nameof(Date)}));
            }
        }
    }
}
