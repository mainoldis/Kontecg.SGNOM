namespace Kontecg
{
    /// <summary>
    /// Data transfer object representing the input information for a subsidy.
    /// </summary>
    public class SubsidioInputDto
    {
        /// <summary>
        /// Gets or sets the request identifier.
        /// </summary>
        public string IdSolicitud { get; set; }

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
        /// Gets or sets the start date.
        /// </summary>
        public string FechaInicio { get; set; }

        /// <summary>
        /// Gets or sets the end date.
        /// </summary>
        public string FechaFinal { get; set; }

        /// <summary>
        /// Gets or sets the number of days.
        /// </summary>
        public string Dias { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public string Importe { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public string Tipo { get; set; }

        /// <summary>
        /// Gets or sets the subtype.
        /// </summary>
        public string Subtipo { get; set; }

        /// <summary>
        /// Gets or sets the percentage.
        /// </summary>
        public string Porciento { get; set; }

        /// <summary>
        /// Gets or sets the waiting days.
        /// </summary>
        public string DiasCarencia { get; set; }

        /// <summary>
        /// Can be set when reading data from excel or when importing data.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// Determines whether the record can be imported based on the Exception property.
        /// </summary>
        /// <returns>True if the record can be imported; otherwise, false.</returns>
        public bool CanBeImported()
        {
            return string.IsNullOrEmpty(Exception);
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            return $"{nameof(IdSolicitud)}: {IdSolicitud}, {nameof(IdPersona)}: {IdPersona}, {nameof(Chapa)}: {Chapa}, {nameof(Trabajador)}: {Trabajador}, {nameof(AreaN1)}: {AreaN1}, {nameof(Area)}: {Area}";
        }
    }
}
