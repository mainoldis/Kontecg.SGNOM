using System;
using Kontecg.Domain.Values;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kontecg.HumanResources
{
    [Serializable]
    public class ClothingSize : ValueObject
    {
        public const string KeyName = "C";

        [StringLength(5)]
        public virtual string Shirt { get; set; } // Camisa o Pulover

        [StringLength(5)]
        public virtual string Pants { get; set; } // Pantalón

        [StringLength(5)]
        public virtual string Overol { get; set; } // Overol

        public virtual int? Footwear { get; set; }

        public virtual int? ProtectionFootwear { get; set; }

        public virtual int? GumBoots { get; set; }

        /// <inheritdoc />
        public ClothingSize()
        {
        }

        /// <inheritdoc />
        public ClothingSize(string shirt, string pants, string overol, int? footwear, int? protectionFootwear, int? gumBoots)
        {
            Shirt = shirt;
            Pants = pants;
            Overol = overol;
            Footwear = footwear;
            ProtectionFootwear = protectionFootwear;
            GumBoots = gumBoots;
        }

        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Shirt;
            yield return Pants;
            yield return Overol;
            yield return Footwear;
            yield return ProtectionFootwear;
            yield return GumBoots;
        }

        public static bool operator ==(ClothingSize one, ClothingSize two)
        {
            // ReSharper disable once PossibleNullReferenceException
            return one.ValueEquals(two);
        }

        public static bool operator !=(ClothingSize one, ClothingSize two)
        {
            // ReSharper disable once PossibleNullReferenceException
            return !one.ValueEquals(two);
        }

        public override string ToString()
        {
            return $"{Shirt}, {Pants}, {Overol}, {Footwear}, {ProtectionFootwear}, {GumBoots}";
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