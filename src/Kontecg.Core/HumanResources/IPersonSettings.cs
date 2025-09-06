using System.Threading.Tasks;

namespace Kontecg.HumanResources
{
    /// <summary>
    /// Used to get settings related to Persons.
    /// </summary>
    public interface IPersonSettings
    {
        bool MustHavePhoto(int? companyId);

        Task<bool> MustHavePhotoAsync(int? companyId);

        bool MustHaveAddress(int? companyId);

        Task<bool> MustHaveAddressAsync(int? companyId);

        bool MustHaveEtnia(int? companyId);

        Task<bool> MustHaveEtniaAsync(int? companyId);

        bool MustHaveClothingSizes(int? companyId);

        Task<bool> MustHaveClothingSizesAsync(int? companyId);
    }
}
