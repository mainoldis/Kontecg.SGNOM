namespace Kontecg
{
    /// <summary>
    /// Data transfer object representing the output information for a subsidy.
    /// </summary>
    public class SubsidioOutputDto
    {
        /// <summary>
        /// Gets or sets the person identifier.
        /// </summary>
        public string IdPersona { get; set; }

        /// <summary>
        /// Gets or sets the employee badge number.
        /// </summary>
        public string Chapa { get; set; }

        /// <summary>
        /// Gets or sets the worker's name.
        /// </summary>
        public string Trabajador { get; set; }

        /// <summary>
        /// Gets or sets the first-level area.
        /// </summary>
        public string AreaN1 { get; set; }

        /// <summary>
        /// Gets or sets the area.
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// Gets or sets the duration of the subsidy.
        /// </summary>
        public string Duration { get; set; }

        /// <summary>
        /// Returns a string representation of the object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{nameof(IdPersona)}: {IdPersona}, {nameof(Chapa)}: {Chapa}, {nameof(Trabajador)}: {Trabajador}, {nameof(AreaN1)}: {AreaN1}, {nameof(Area)}: {Area}, {nameof(Duration)}: {Duration}";
        }
    }
}
