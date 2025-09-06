using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.Threading.Tasks;
using System.Threading;
using System;
using Kontecg.Workflows;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Kontecg.Salary;

namespace Kontecg.EFCore.ValueGenerators
{
    /// <summary>  
    /// Generates unique codes for TimeDistributionDocument entities based on the year and existing codes.  
    /// </summary>  
    public class TimeDistributionDocumentCodeGenerator : ValueGenerator<string>
    {
        /// <inheritdoc />  
        public override bool GeneratesTemporaryValues => false;

        /// <summary>  
        /// Generates the next unique code for the given entity entry.  
        /// </summary>  
        /// <param name="entry">The entity entry for which the code is being generated.</param>  
        /// <returns>The generated unique code.</returns>  
        /// <exception cref="InvalidOperationException">Thrown if the entity does not implement IMustHaveReview.</exception>  
        public override string Next(EntityEntry entry)
        {
            if (entry.Entity is not IMustHaveReview doc)
                throw new InvalidOperationException("The entity must implement IMustHaveReview.");

            if (!string.IsNullOrWhiteSpace(doc.Code) && doc.Code != "AUTO")
                return doc.Code;

            var context = entry.Context;
            var year = doc.MadeOn.ToString("yy");

            var maxCode = GetMaxCodeInTransaction(context, year);
            return GenerateNextCode(maxCode, year);
        }

        /// <summary>  
        /// Asynchronously generates the next unique code for the given entity entry.  
        /// </summary>  
        /// <param name="entry">The entity entry for which the code is being generated.</param>  
        /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>  
        /// <returns>A task that represents the asynchronous operation, containing the generated unique code.</returns>  
        public override ValueTask<string> NextAsync(EntityEntry entry, CancellationToken cancellationToken = default)
        {
            return new ValueTask<string>(Next(entry));
        }

        /// <summary>  
        /// Retrieves the maximum code value for TimeDistributionDocument entities in the current transaction.  
        /// </summary>  
        /// <param name="context">The database context.</param>  
        /// <param name="year">The year used as part of the code.</param>  
        /// <returns>The maximum code value found.</returns>  
        protected virtual string GetMaxCodeInTransaction(DbContext context, string year)
        {
            var suffix = $".{year}";

            var maxDbCode = context.Set<TimeDistributionDocument>()
                                   .AsNoTracking()
                                   .Where(e => e.Code.EndsWith(suffix))
                                   .Select(e => ExtractNumericValue(e.Code, suffix))
                                   .DefaultIfEmpty(0)
                                   .Max();

            var pendingMax = context.ChangeTracker.Entries<TimeDistributionDocument>()
                                    .Where(e => e.State == EntityState.Added && e.Entity.Code?.EndsWith(suffix) == true)
                                    .Select(e => ExtractNumericValue(e.Entity.Code, suffix))
                                    .DefaultIfEmpty(0)
                                    .Max();

            return $"{Math.Max(maxDbCode, pendingMax)}{suffix}";
        }

        /// <summary>  
        /// Generates the next code based on the maximum code value and the year.  
        /// </summary>  
        /// <param name="maxCode">The maximum code value found.</param>  
        /// <param name="year">The year used as part of the code.</param>  
        /// <returns>The next unique code.</returns>  
        protected virtual string GenerateNextCode(string maxCode, string year)
        {
            var parts = maxCode.Split('.');
            return parts.Length == 2 && parts[1] == year && int.TryParse(parts[0], out var lastNumber)
                ? $"{(lastNumber + 1):000}.{year}"
                : $"001.{year}";
        }

        /// <summary>  
        /// Extracts the numeric value from a code string based on the given suffix.  
        /// </summary>  
        /// <param name="code">The code string.</param>  
        /// <param name="suffix">The suffix used to identify the numeric part.</param>  
        /// <returns>The extracted numeric value, or 0 if extraction fails.</returns>  
        protected virtual long ExtractNumericValue(string code, string suffix)
        {
            if (code.EndsWith(suffix))
            {
                var numericPart = code[..^suffix.Length].TakeWhile(char.IsDigit);
                return long.TryParse(string.Concat(numericPart), out var result) ? result : 0;
            }
            return 0;
        }
    }
}