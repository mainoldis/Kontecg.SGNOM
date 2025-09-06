using Kontecg.Logging;
using System.Runtime.Serialization;
using System;

namespace Kontecg.Timing
{
    public class KontecgInvalidPeriodException : KontecgException, IHasLogSeverity
    {
        /// <summary>
        ///     Default log severity
        /// </summary>
        public static LogSeverity DefaultLogSeverity = LogSeverity.Warn;

        /// <summary>
        ///     Creates a new <see cref="KontecgInvalidPeriodException" /> object.
        /// </summary>
        public KontecgInvalidPeriodException()
        {
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgInvalidPeriodException" /> object.
        /// </summary>
        public KontecgInvalidPeriodException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgInvalidPeriodException" /> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public KontecgInvalidPeriodException(string message)
            : base(message)
        {
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        ///     Creates a new <see cref="KontecgInvalidPeriodException" /> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public KontecgInvalidPeriodException(string message, Exception innerException)
            : base(message, innerException)
        {
            Severity = DefaultLogSeverity;
        }

        /// <summary>
        ///     Severity of the exception.
        ///     Default: Warn.
        /// </summary>
        public LogSeverity Severity { get; set; }
    }
}
