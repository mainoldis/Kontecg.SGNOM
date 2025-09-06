using System;
using System.Reflection;

namespace Kontecg.HumanResources
{
    /// <summary>
    ///     Used to identify a person.
    /// </summary>
    [Serializable]
    public class PersonIdentifier : IPersonIdentifier
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PersonIdentifier" /> class.
        /// </summary>
        /// <param name="companyId">Company Id of the person.</param>
        /// <param name="personId">Id of the person.</param>
        public PersonIdentifier(int? companyId, long personId)
        {
            CompanyId = companyId;
            PersonId = personId;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PersonIdentifier" /> class.
        /// </summary>
        protected PersonIdentifier()
        {
        }

        public int? CompanyId { get; protected set; }

        public long PersonId { get; protected set; }

        public string ToPersonIdentifierString()
        {
            if (CompanyId == null) return PersonId.ToString();

            return PersonId + "@" + CompanyId;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PersonIdentifier)) return false;

            //Same instances must be considered as equal
            if (ReferenceEquals(this, obj)) return true;

            //Transient objects are not considered as equal
            var other = (PersonIdentifier)obj;

            //Must have a IS-A relation of types or must be same type
            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) &&
                !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis)) return false;

            return CompanyId == other.CompanyId && PersonId == other.PersonId;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hash = 17;
            hash = CompanyId.HasValue ? hash * 23 + CompanyId.GetHashCode() : hash;
            hash = hash * 23 + PersonId.GetHashCode();
            return hash;
        }

        public override string ToString()
        {
            return ToPersonIdentifierString();
        }

        /// <inheritdoc />
        public static bool operator ==(PersonIdentifier left, PersonIdentifier right)
        {
            if (Equals(left, null)) return Equals(right, null);

            return left.Equals(right);
        }

        /// <inheritdoc />
        public static bool operator !=(PersonIdentifier left, PersonIdentifier right)
        {
            return !(left == right);
        }
    }
}
