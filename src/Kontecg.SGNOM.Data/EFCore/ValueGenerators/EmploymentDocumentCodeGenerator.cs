using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kontecg.WorkRelations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Kontecg.EFCore.ValueGenerators
{
    public class EmploymentDocumentCodeGenerator : ValueGenerator<string>
    {
        /// <inheritdoc />
        public override bool GeneratesTemporaryValues => false;

        /// <inheritdoc />
        public override string Next(EntityEntry entry)
        {
            if (entry.Entity is not EmploymentDocument employmentDoc)
                throw new InvalidOperationException("La entidad debe ser de tipo EmploymentDocument");

            if (!string.IsNullOrWhiteSpace(employmentDoc.Code) && employmentDoc.Code != "AUTO")
                return employmentDoc.Code;

            var context = entry.Context;
            var type = employmentDoc.Type;
            var year = employmentDoc.EffectiveSince.ToString("yy");

            var maxCode = GetMaxCodeInTransaction(context, year, type);
            return GenerateNextCode(maxCode, year, type);
        }

        public override ValueTask<string> NextAsync(EntityEntry entry, CancellationToken cancellationToken = default)
        {
            return new ValueTask<string>(Next(entry));
        }

        private string GetMaxCodeInTransaction(DbContext context, string year, EmploymentType type)
        {
            var prefix = $"{year}.{type}.";
            var maxDbCode = context.Set<EmploymentDocument>()
                                   .AsNoTracking()
                                   .Where(e => e.Type == type && e.Code.StartsWith(prefix))
                                   .Select(e => ExtractNumericValue(e.Code, prefix))
                                   .DefaultIfEmpty(0)
                                   .Max();

            var pendingMax = context.ChangeTracker.Entries<EmploymentDocument>()
                                    .Where(e => e.State == EntityState.Added && e.Entity.Code?.StartsWith(prefix) == true)
                                    .Select(e => ExtractNumericValue(e.Entity.Code, prefix))
                                    .DefaultIfEmpty(0)
                                    .Max();

            return $"{prefix}{Math.Max(maxDbCode, pendingMax)}";
        }

        private string GenerateNextCode(string maxCode, string year, EmploymentType type)
        {
            var parts = maxCode.Split('.');
            return parts.Length == 3 && parts[0] == year && parts[1] == type.ToString() && int.TryParse(parts[2], out var lastNumber)
                ? $"{year}.{type}.{(lastNumber + 1):000}"
                : $"{year}.{type}.001";
        }

        private long ExtractNumericValue(string code, string prefix)
        {
            if (code.StartsWith(prefix))
            {
                var numericPart = code.Substring(prefix.Length).TakeWhile(char.IsDigit);
                return long.TryParse(string.Concat(numericPart), out var result) ? result : 0;
            }
            return 0;
        }
    }
}