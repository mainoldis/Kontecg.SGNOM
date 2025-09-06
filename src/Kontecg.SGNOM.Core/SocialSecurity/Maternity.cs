using System;

namespace Kontecg.SocialSecurity
{
    public class Maternity : SubsidyDocument
    {
        public virtual bool MultiplePregnancy { get; set; }

        public virtual DateTime DueDate { get; set; }
    }
}
