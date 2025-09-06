using System;
using Kontecg.Domain.Values;
using Kontecg.Primitives;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.Linq;

namespace Kontecg.HumanResources
{
    /// <summary>
    /// Represents ethnic and physical characteristics of a person in the human resources system.
    /// This value object encapsulates demographic and physical attributes that may be relevant
    /// for organizational diversity tracking and personal identification purposes.
    /// </summary>
    /// <remarks>
    /// The Etnia class is implemented as a value object, meaning it is immutable and its identity
    /// is based on its property values rather than a unique identifier. It inherits from ValueObject
    /// to provide proper equality semantics and value-based comparison capabilities.
    /// </remarks>
    [Serializable]
    public class Etnia : ValueObject
    {
        /// <summary>
        /// Gets or sets the height of the person in the specified unit of measurement.
        /// </summary>
        /// <value>
        /// The height as a decimal value, or null if not specified. The unit of measurement
        /// should be consistent across the application (typically centimeters or inches).
        /// </value>
        /// <remarks>
        /// Height is stored as a nullable decimal to accommodate cases where this information
        /// is not available or not required for the specific use case.
        /// </remarks>
        public virtual decimal? Height { get; set; }

        /// <summary>
        /// Gets or sets the racial or ethnic classification of the person.
        /// </summary>
        /// <value>
        /// A Race enumeration value indicating the person's racial or ethnic background,
        /// or null if not specified.
        /// </value>
        /// <remarks>
        /// This property is used for diversity tracking and demographic analysis purposes.
        /// The classification should follow organizational or legal standards for racial
        /// and ethnic categorization.
        /// </remarks>
        public virtual Race? Race { get; set; }

        /// <summary>
        /// Gets or sets the civil status or marital status of the person.
        /// </summary>
        /// <value>
        /// A string representing the person's civil status (e.g., "Single", "Married", "Divorced"),
        /// with a maximum length of 25 characters.
        /// </value>
        /// <remarks>
        /// Civil status is important for various HR processes including benefits administration,
        /// tax calculations, and emergency contact information.
        /// </remarks>
        [StringLength(25)]
        public virtual string CivilStatus { get; set; }

        /// <summary>
        /// Gets or sets the eye color of the person.
        /// </summary>
        /// <value>
        /// An EyeColor enumeration value indicating the person's eye color, or null if not specified.
        /// </value>
        /// <remarks>
        /// Eye color is typically used for identification purposes and may be required
        /// for certain types of official documentation or security clearances.
        /// </remarks>
        public virtual EyeColor? EyeColor { get; set; }

        /// <summary>
        /// Initializes a new instance of the Etnia class with default values.
        /// </summary>
        /// <remarks>
        /// This constructor creates an Etnia instance with all properties set to null,
        /// allowing for gradual population of the ethnic characteristics.
        /// </remarks>
        /// <inheritdoc />
        public Etnia()
        {
        }

        /// <summary>
        /// Initializes a new instance of the Etnia class with the specified values.
        /// </summary>
        /// <param name="height">The height of the person, or null if not specified.</param>
        /// <param name="race">The racial or ethnic classification, or null if not specified.</param>
        /// <param name="civilStatus">The civil status of the person, or null if not specified.</param>
        /// <param name="eyeColor">The eye color of the person, or null if not specified.</param>
        /// <remarks>
        /// This constructor allows for complete initialization of all ethnic characteristics
        /// in a single operation, which is useful when all information is available at creation time.
        /// </remarks>
        /// <inheritdoc />
        public Etnia(decimal? height, Race? race, string civilStatus, EyeColor? eyeColor)
        {
            Height = height;
            Race = race;
            CivilStatus = civilStatus;
            EyeColor = eyeColor;
        }

        /// <summary>
        /// Gets the atomic values that constitute this value object's identity.
        /// </summary>
        /// <returns>
        /// An enumerable collection of objects representing the individual property values
        /// that determine the equality of this Etnia instance.
        /// </returns>
        /// <remarks>
        /// This method is used by the base ValueObject class to implement proper equality
        /// comparison. The returned values are used to determine if two Etnia instances
        /// are considered equal based on their property values rather than reference equality.
        /// </remarks>
        protected override IEnumerable<object> GetAtomicValues()
        {
            // Using a yield return statement to return each element one at a time
            yield return Height;
            yield return Race;
            yield return CivilStatus;
            yield return EyeColor;
        }

        /// <summary>
        /// Determines whether two Etnia instances are equal based on their property values.
        /// </summary>
        /// <param name="one">The first Etnia instance to compare.</param>
        /// <param name="two">The second Etnia instance to compare.</param>
        /// <returns>True if the instances have equal property values; otherwise, false.</returns>
        /// <remarks>
        /// This operator uses the ValueEquals method from the base ValueObject class to perform
        /// value-based comparison rather than reference comparison.
        /// </remarks>
        public static bool operator ==(Etnia one, Etnia two)
        {
            // ReSharper disable once PossibleNullReferenceException
            return one.ValueEquals(two);
        }

        /// <summary>
        /// Determines whether two Etnia instances are not equal based on their property values.
        /// </summary>
        /// <param name="one">The first Etnia instance to compare.</param>
        /// <param name="two">The second Etnia instance to compare.</param>
        /// <returns>True if the instances have different property values; otherwise, false.</returns>
        /// <remarks>
        /// This operator is the logical negation of the equality operator and uses the same
        /// value-based comparison logic.
        /// </remarks>
        public static bool operator !=(Etnia one, Etnia two)
        {
            // ReSharper disable once PossibleNullReferenceException
            return !one.ValueEquals(two);
        }

        /// <summary>
        /// Returns a string representation of the Etnia instance.
        /// </summary>
        /// <returns>
        /// A string containing the values of all properties in a readable format.
        /// </returns>
        /// <remarks>
        /// The string format includes all property values separated by commas, which is useful
        /// for debugging and logging purposes. Null values are included in the string representation.
        /// </remarks>
        public override string ToString()
        {
            return $"{Height}, {Race}, {CivilStatus}, {EyeColor}";
        }

        /// <summary>
        /// Returns a hash code for this Etnia instance based on its property values.
        /// </summary>
        /// <returns>
        /// A hash code that is consistent with the equality comparison of this value object.
        /// </returns>
        /// <remarks>
        /// The hash code is calculated by combining the hash codes of all atomic values using
        /// the XOR operation. This ensures that objects with equal values have equal hash codes,
        /// which is important for proper behavior in hash-based collections.
        /// </remarks>
        public override int GetHashCode()
        {
            var thisValues = GetAtomicValues();
            return thisValues
                .Select(x => (x?.GetHashCode()) ?? 0)
                .Aggregate((x, y) => x ^ y);
        }

        /// <summary>
        /// Determines whether this Etnia instance is equal to the specified object.
        /// </summary>
        /// <param name="obj">The object to compare with this instance.</param>
        /// <returns>True if the object is an Etnia with equal property values; otherwise, false.</returns>
        /// <remarks>
        /// This method delegates to the ValueEquals method from the base ValueObject class,
        /// which performs proper type checking and value-based comparison.
        /// </remarks>
        public override bool Equals(object obj)
        {
            return ValueEquals(obj);
        }
    }
}