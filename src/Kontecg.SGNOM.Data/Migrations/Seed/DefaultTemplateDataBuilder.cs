using System;
using System.IO;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using Kontecg.Domain;
using Kontecg.EFCore;
using Kontecg.Json;
using Kontecg.MultiCompany;
using Kontecg.Organizations;
using Kontecg.Salary;
using Kontecg.Timing;
using Kontecg.Workflows;
using Microsoft.EntityFrameworkCore;

namespace Kontecg.Migrations.Seed
{
    public class DefaultTemplateDataBuilder
    {
        private readonly SGNOMDbContext _context;
        private readonly IContentFolders _contentFolders;
        private readonly ILogger _logger;

        public DefaultTemplateDataBuilder(SGNOMDbContext context, IContentFolders contentFolders, ILogger logger)
        {
            _context = context;
            _contentFolders = contentFolders;
            _logger = logger;
        }

        public void Create()
        {
            try
            {
                var companyId = KontecgCoreConsts.MultiCompanyEnabled ? MultiCompanyConsts.DefaultCompanyId : 1;

                var document = _context.TemplateDocuments.IgnoreQueryFilters()
                    .FirstOrDefault(r => r.CompanyId == companyId && (r.Review == ReviewStatus.Reviewed || r.Review == ReviewStatus.Confirmed));

                if (document == null)
                {
                    document = new TemplateDocument(0, Clock.Now, 1, 1, companyId) { Review = ReviewStatus.Reviewed };
                    _context.TemplateDocuments.Add(document);
                    _context.SaveChanges();
                }

                string tempDataFile = Path.GetFullPath(Path.Combine(_contentFolders.DataFolder, "seed", "template.json"));

                if (!File.Exists(tempDataFile))
                    _logger.WarnFormat("File '{0}' not found.", tempDataFile);
                else
                {
                    var content = File.ReadAllText(tempDataFile, Encoding.UTF8);
                    if (!string.IsNullOrEmpty(content))
                    {
                        var template = content.FromJsonString<TemplateRecord[]>();
                        for (int index = 0; index < template.Length; index++)
                        {
                            TemplateRecord item = template[index];
                            AddTemplateIfNotExists(document, item.OrganizationUnitCode, item.CenterCost,
                                item.OccupationCode, item.Scholarship, item.Proposals, workShift: item.WorkShift);
                        }
                    }
                }

                _logger.Info("Template related data created.");
            }
            catch (Exception ex)
            {
                _logger.Warn("Template couldn't be saved.", ex);
            }
        }

        private void AddTemplateIfNotExists(TemplateDocument document, string organizationUnitCode, int centerCost, string occupationCode, string scholarship = null, int proposals = 1, EmployeeSalaryForm employeeSalaryForm = EmployeeSalaryForm.Royal, params string[] workShift)
        {
            if (_context.Templates.IgnoreQueryFilters()
                .Include(o => o.Occupation)
                .Include(s => s.ScholarshipLevel)
                .Any(s =>
                    s.CompanyId == document.CompanyId && s.DocumentId == document.Id && s.OrganizationUnitCode == organizationUnitCode &&
                    s.Occupation.Code == occupationCode && s.CenterCost == centerCost && s.ScholarshipLevel.Acronym == scholarship))
                return;

            var occupation = _context.Occupations.IgnoreQueryFilters().FirstOrDefault(o => o.Code == occupationCode);
            if (occupation == null) return;

            var level = _context.ScholarshipLevelDefinitions.IgnoreQueryFilters()
                .FirstOrDefault(s => s.Acronym == scholarship && s.Scope == ScopeData.Company);

            if (workShift.Length == 0)
            {
                Array.Resize(ref workShift, 1);
                workShift[0] = "N";
            }

            if (workShift.Length > proposals) return;

            var template = new Template
            {
                DocumentId = document.Id,
                CompanyId = document.CompanyId,
                OrganizationUnitId = 0,
                OrganizationUnitCode = organizationUnitCode,
                CenterCost = centerCost,
                EmployeeSalaryForm = employeeSalaryForm,
                OccupationId = occupation.Id,
                ScholarshipLevel = level,
                WorkShift = workShift.ToJsonString(),
                Proposals = proposals,
                Approved = proposals
            };

            _context.Templates.Add(template);
            _context.SaveChanges();

            AddJobPositionIfNotExists(template);
        }

        private void AddJobPositionIfNotExists(Template template)
        {
            var count = (_context.TemplateJobPositions.IgnoreQueryFilters().Count(s => s.TemplateId == template.Id));
            if (count >= template.Approved) return;

            string[] workShiftPattern = template.WorkShift.FromJsonString<string[]>();
            WorkShift[] workShifts = _context.WorkShifts.IgnoreQueryFilters().Where(w => workShiftPattern.Contains(w.DisplayName)).ToArray();

            for (int i = 1; i <= (template.Approved - count); i++)
            {
                var jobPosition = new TemplateJobPosition
                {
                    TemplateId = template.Id,
                    CompanyId = template.CompanyId,
                    OrganizationUnitId = template.OrganizationUnitId,
                    OrganizationUnitCode = template.OrganizationUnitCode,
                    CenterCost = template.CenterCost,
                    EmployeeSalaryForm = template.EmployeeSalaryForm,
                    OccupationId = template.OccupationId,
                    ScholarshipLevelId = template.ScholarshipLevelId,
                    WorkShift = (workShiftPattern.Length == 1 ? workShifts[0] : workShifts[count + i - 1]),
                    Code = (count + i).ToString(new string('0', TemplateJobPosition.MaxCodeLength))
                };

                _context.TemplateJobPositions.Add(jobPosition);
                _context.SaveChanges();
            }
        }

        private record TemplateRecord(
            string OrganizationUnitCode,
            int CenterCost,
            string OccupationCode,
            string Scholarship,
            int Proposals,
            string[] WorkShift);
    }
}
