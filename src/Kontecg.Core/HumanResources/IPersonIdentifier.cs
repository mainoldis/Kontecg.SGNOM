namespace Kontecg.HumanResources
{
    public interface IPersonIdentifier
    {
        /// <summary>
        ///     Company Id. Can be null for host users.
        /// </summary>
        int? CompanyId { get; }

        /// <summary>
        ///     Id of the person.
        /// </summary>
        long PersonId { get; }
    }
}
