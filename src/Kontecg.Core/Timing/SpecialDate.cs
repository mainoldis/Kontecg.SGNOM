using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using Kontecg.Domain.Entities;

namespace Kontecg.Timing
{
    [Table("special_dates", Schema = "gen")]
    public class SpecialDate : Entity
    {
        [Required]
        public virtual DateTime Date { get; set; }

        [Required]
        public virtual DayDecorator Cause { get; set; }

        public SpecialDate()
        {
            Cause = DayDecorator.BreakSaturday;
        }

        public SpecialDate(DateTime date)
            : this()
        {
            Date = date;
        }

        public SpecialDate(DateTime date, DayDecorator cause)
        {
            Date = date;
            Cause = cause;
        }
    }
}
