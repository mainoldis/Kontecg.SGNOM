using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Kontecg.Domain.Values;
using Kontecg.Extensions;

namespace Kontecg.HumanResources
{
    /// <summary>
    ///     Register an address object.
    /// </summary>
    [Serializable]
    public class Address : ValueObject
    {
        public const string KeyName = "D";

        public const int MaxStreetLength = 450;

        public const int MaxZipCodeLength = 20;

        [StringLength(MaxStreetLength)]
        public string Street { get; set; }

        [StringLength(HumanResources.City.MaxCityNameLength)]
        public string City { get; set; }

        [StringLength(HumanResources.State.MaxStateNameLength)]
        public string State { get; set; }

        [Required]
        [StringLength(HumanResources.Country.MaxCountryNameLength)]
        public string Country { get; set; }

        [StringLength(MaxZipCodeLength)]
        public string ZipCode { get; set; }

        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Phone]
        [StringLength(20)]
        public string Fax { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

        public Address()
        {
            Country = "Cuba";
        }

        public Address(string street, string city, string state, string country, string zipCode = null, string phoneNumber = null, string fax = null, double latitude = 0, double longitude = 0 )
        {
            Street = street;
            City = city;
            State = state;
            Country = country;
            ZipCode = zipCode;
            PhoneNumber = phoneNumber;
            Fax = fax;
            Latitude = latitude;
            Longitude = longitude;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Street;
            yield return City;
            yield return State;
            yield return Country;
            yield return ZipCode;
            yield return PhoneNumber;
            yield return Fax;
            yield return Latitude;
            yield return Longitude;
        }

        public static bool operator ==(Address one, Address two)
        {
            // ReSharper disable once PossibleNullReferenceException
            return one.ValueEquals(two);
        }

        public static bool operator !=(Address one, Address two)
        {
            // ReSharper disable once PossibleNullReferenceException
            return !one.ValueEquals(two);
        }

        public override string ToString()
        {
            return
                $"{Street}, {City}, {State}, {Country}{(!ZipCode.IsNullOrWhiteSpace() ? ", CP: " + ZipCode : "")}{(!PhoneNumber.IsNullOrWhiteSpace() ? ", T: " + PhoneNumber : "")}{(!Fax.IsNullOrWhiteSpace() ? ", F: " + Fax : "")}";
        }

        public override int GetHashCode()
        {
            var thisValues = GetAtomicValues();
            return thisValues
                .Select(x => (x?.GetHashCode()) ?? 0)
                .Aggregate((x, y) => x ^ y);
        }

        public override bool Equals(object obj)
        {
            return ValueEquals(obj);
        }
    }
}
