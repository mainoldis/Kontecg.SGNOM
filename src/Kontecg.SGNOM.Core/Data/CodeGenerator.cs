using System;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Dependency;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.Extensions;
using Kontecg.Holidays;
using Kontecg.Runtime.Session;
using Kontecg.Salary;
using Kontecg.Workflows;
using Kontecg.WorkRelations;

namespace Kontecg.Data
{
    public class CodeGenerator : ICodeGenerator, ISingletonDependency
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IEmploymentRepository _employmentRepository;
        private readonly IRepository<TimeDistributionDocument> _timeDistributionDocumentRepository;
        private readonly IRepository<HolidayDocument, long> _holidayRepository;
        private readonly IGuidGenerator _sequentialGuidGenerator;

        public CodeGenerator(
            IUnitOfWorkManager unitOfWorkManager,
            IEmploymentRepository employmentRepository,
            IRepository<TimeDistributionDocument> timeDistributionDocumentRepository,
            IRepository<HolidayDocument, long> holidayRepository,
            IGuidGenerator sequentialGuidGenerator)
        {
            _employmentRepository = employmentRepository;
            _sequentialGuidGenerator = sequentialGuidGenerator;
            _holidayRepository = holidayRepository;
            _timeDistributionDocumentRepository = timeDistributionDocumentRepository;
            _unitOfWorkManager = unitOfWorkManager;
            KontecgSession = NullKontecgSession.Instance;
        }

        /// <summary>
        ///     Reference to the session.
        /// </summary>
        public IKontecgSession KontecgSession { get; set; }

        public virtual string CreateEmploymentCode(DateTime effectiveDate, EmploymentType relationType)
        {
            return $"{effectiveDate:yy}.{relationType}.{ExtractMaxEmploymentNumber($"{effectiveDate:yy}.{relationType}."):000}";
        }

        public virtual async Task<string> CreateEmploymentCodeAsync(DateTime effectiveDate, EmploymentType relationType)
        {
            return $"{effectiveDate:yy}.{relationType}.{await ExtractMaxEmploymentNumberAsync($"{effectiveDate:yy}.{relationType}."):000}";
        }

        public virtual string CreateTimeDistributionDocumentCode(DateTime workTime)
        {
            return $"{ExtractMaxTimeDistributionDocumentNumber($".{workTime:yy}")}.{workTime:yy}";
        }

        public virtual async Task<string> CreateTimeDistributionDocumentCodeAsync(DateTime workTime)
        {
            return $"{ExtractMaxTimeDistributionDocumentNumberAsync($".{workTime:yy}")}.{workTime:yy}";
        }

        public string CreateHolidayDocumentCode(DateTime workTime)
        {
            return $"{ExtractMaxHolidayDocumentNumber($".{workTime:yy}")}.{workTime:yy}";
        }

        public async Task<string> CreateHolidayDocumentCodeAsync(DateTime workTime)
        {
            return $"{ExtractMaxHolidayDocumentNumberAsync($".{workTime:yy}")}.{workTime:yy}";
        }

        public virtual Guid CreateOrUpdateEmploymentGroupId(Guid? id = null)
        {
            //RULE: El ID de empleo será para definir agrupaciones de movimientos de nóminas a partir de un alta, re-ubicaciones y baja
            return _sequentialGuidGenerator.Create();
        }

        public virtual Task<Guid> CreateOrUpdateEmploymentGroupIdAsync(Guid? id = null)
        {
            return Task.FromResult(_sequentialGuidGenerator.Create());
        }

        protected virtual int ExtractMaxEmploymentNumber(string part)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    var e = _employmentRepository.GetAllList(c => c.Code.StartsWith(part) && (c.Review == ReviewStatus.Reviewed || c.Review == ReviewStatus.Confirmed))
                        .Select(n => int.Parse(n.Code.RemovePreFix(part))).ToList();
                    return e.Count > 0 ? e.Max() + 1 : 1;
                }
            });
        }

        protected virtual async Task<int> ExtractMaxEmploymentNumberAsync(string part)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    var e = (await _employmentRepository.GetAllListAsync(c => c.Code.StartsWith(part) && (c.Review == ReviewStatus.Reviewed || c.Review == ReviewStatus.Confirmed)))
                        .Select(n => int.Parse(n.Code.RemovePreFix(part))).ToList();
                    return e.Count > 0 ? e.Max() + 1 : 1;
                }
            });
        }

        protected virtual int ExtractMaxTimeDistributionDocumentNumber(string part)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    var e = _timeDistributionDocumentRepository.GetAllList(c => c.Code.EndsWith(part) && (c.Review == ReviewStatus.Reviewed || c.Review == ReviewStatus.Confirmed))
                        .Select(n => int.Parse(n.Code.RemovePreFix(part))).ToList();
                    return e.Count > 0 ? e.Max() + 1 : 1;
                }
            });
        }

        protected virtual async Task<int> ExtractMaxTimeDistributionDocumentNumberAsync(string part)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    var e = (await _timeDistributionDocumentRepository.GetAllListAsync(c => c.Code.EndsWith(part) && (c.Review == ReviewStatus.Reviewed || c.Review == ReviewStatus.Confirmed)))
                        .Select(n => int.Parse(n.Code.RemovePreFix(part))).ToList();
                    return e.Count > 0 ? e.Max() + 1 : 1;
                }
            });
        }

        protected virtual int ExtractMaxHolidayDocumentNumber(string part)
        {
            return _unitOfWorkManager.WithUnitOfWork(() =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    var e = _holidayRepository.GetAllList(c => c.Code.EndsWith(part) && (c.Review == ReviewStatus.Reviewed || c.Review == ReviewStatus.Confirmed))
                        .Select(n => int.Parse(n.Code.RemovePreFix(part))).ToList();
                    return e.Count > 0 ? e.Max() + 1 : 1;
                }
            });
        }

        protected virtual async Task<int> ExtractMaxHolidayDocumentNumberAsync(string part)
        {
            return await _unitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (_unitOfWorkManager.Current.SetCompanyId(KontecgSession.CompanyId))
                {
                    var e = (await _holidayRepository.GetAllListAsync(c => c.Code.EndsWith(part) && (c.Review == ReviewStatus.Reviewed || c.Review == ReviewStatus.Confirmed)))
                        .Select(n => int.Parse(n.Code.RemovePreFix(part))).ToList();
                    return e.Count > 0 ? e.Max() + 1 : 1;
                }
            });
        }
    }
}
