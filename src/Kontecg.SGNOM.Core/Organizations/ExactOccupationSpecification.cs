using System;
using System.Linq.Expressions;
using Kontecg.Specifications;

namespace Kontecg.Organizations
{
    public class ExactOccupationSpecification : Specification<Occupation>
    {
        public Occupation Specification { get; }

        public ExactOccupationSpecification(Occupation specification)
        {
            Specification = specification;
        }

        public override Expression<Func<Occupation, bool>> ToExpression()
        {
            return t => Specification != null
                        && (t.Code == Specification.Code
                            ||
                            (t.DisplayName == Specification.DisplayName
                             && t.CategoryId == Specification.CategoryId
                             && t.IsActive
                             && t.GroupId == Specification.GroupId
                             && t.ResponsibilityId == Specification.ResponsibilityId));
        }
    }
}
