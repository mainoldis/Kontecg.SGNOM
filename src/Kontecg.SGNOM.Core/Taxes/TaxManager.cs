using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Kontecg.Accounting;
using Kontecg.Domain.Repositories;
using Kontecg.Domain.Uow;
using Kontecg.HumanResources;
using Kontecg.Runtime.Session;
using Kontecg.Salary;
using Kontecg.UI;
using NMoneys;

namespace Kontecg.Taxes
{
    public class TaxManager : SGNOMDomainServiceBase, ITaxManager
    {
        private readonly ITaxSettingStore _settingStore;
        private readonly IRepository<AccountingTaxSummary, long> _taxSummaryRepository;
        private readonly IRepository<PersonTax> _personTaxRepository;
        private IReadOnlyDictionary<TaxType, TaxInfo> _taxSetting;

        /// <inheritdoc />
        public TaxManager(
            ITaxSettingStore settingStore, 
            IRepository<AccountingTaxSummary, long> taxSummaryRepository, 
            IRepository<PersonTax> personTaxRepository)
        {
            _settingStore = settingStore;
            _taxSummaryRepository = taxSummaryRepository;
            _personTaxRepository = personTaxRepository;

            KontecgSession = NullKontecgSession.Instance;
        }

        public IKontecgSession KontecgSession { get; set; }

        /// <inheritdoc />
        public void CreateRange(TaxType type, TaxRangeRecord range)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public async Task CreateRangeAsync(TaxType type, TaxRangeRecord range)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void CreateTaxForPerson(PersonTax taxPerson)
        {
            UnitOfWorkManager.WithUnitOfWork(() =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
                {
                    if (_personTaxRepository.FirstOrDefault(p =>
                            p.PersonId == taxPerson.PersonId && p.GroupId == taxPerson.GroupId && p.TaxType == taxPerson.TaxType) != null)
                        throw new UserFriendlyException(L("TaxForPersonIsAlreadyExists"));

                    _personTaxRepository.Insert(taxPerson);
                }
            });
        }

        /// <inheritdoc />
        public async Task CreateTaxForPersonAsync(PersonTax taxPerson)
        {
            await UnitOfWorkManager.WithUnitOfWorkAsync(async () =>
            {
                using (CurrentUnitOfWork.SetCompanyId(KontecgSession.CompanyId))
                {
                    if (await _personTaxRepository.FirstOrDefaultAsync(p =>
                            p.PersonId == taxPerson.PersonId && p.GroupId == taxPerson.GroupId &&
                            p.CompanyId == taxPerson.CompanyId && p.TaxType == taxPerson.TaxType) != null)
                        throw new UserFriendlyException(L("TaxForPersonIsAlreadyExists"));

                    await _personTaxRepository.InsertAsync(taxPerson);
                }
            });
        }

        /// <inheritdoc />
        public decimal Calculate(long personId, Guid groupId, TaxType taxType, decimal amount, decimal discount = 0)
        {
            _taxSetting = _taxSetting ??= _settingStore.GetTaxesInfo(KontecgSession.CompanyId);
            decimal taxValue = 0;
            var info = _taxSetting[taxType];
            var rangesToApply = info.Ranges.OrderBy(o => o.Minimum).ToList();

            if (rangesToApply.Count > 0)
                for (int i = 0; i < rangesToApply.Count; i++)
                {
                    if (amount > rangesToApply[i].Minimum && amount > rangesToApply[i].Maximum)
                    {
                        taxValue += (rangesToApply[i].Maximum - rangesToApply[i].Minimum) * rangesToApply[i].Percent * 0.01M;
                    }
                    else if (amount > rangesToApply[i].Minimum && amount <= rangesToApply[i].Maximum)
                    {
                        taxValue += (amount - rangesToApply[i].Minimum) * rangesToApply[i].Percent * 0.01M;
                        break;
                    }
                }
            else
                taxValue = amount;

            var personalTax = info.Persons.FirstOrDefault(tp => tp.PersonId == personId && tp.GroupId == groupId);
            if (personalTax == null) return taxValue * info.Percent * 0.01M - discount;

            switch (personalTax.MathType)
            {
                case MathType.Percent:
                    taxValue *= personalTax.Factor * 0.01M;
                    break;
                case MathType.MinimumWage:
                    taxValue = personalTax.Factor;
                    break;
                case MathType.Formula:
                    //var lexer = new FormulaLexer(personalTax.Formula);
                    //var tokens = lexer.Tokenize();
                    //var parser = new FormulaParser(tokens);
                    //var ast = parser.ParseExpression();
                    //var registry = new InstructionRegistry();
                    //var evaluator = new FormulaEvaluator(registry);
                    //var result = evaluator.Evaluate(ast);
                    //return (decimal) result;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return taxValue * info.Percent * 0.01M - discount;
        }

        /// <inheritdoc />
        public Money Calculate(long personId, Guid groupId, TaxType taxType, Money amount, Money? discount = null)
        {
            _taxSetting = _taxSetting ??= _settingStore.GetTaxesInfo(KontecgSession.CompanyId);
            decimal taxValue = 0;
            var info = _taxSetting[taxType];
            var rangesToApply = info.Ranges.OrderBy(o => o.Minimum).ToList();
            if (rangesToApply.Count > 0)
                for (int i = 0; i < rangesToApply.Count; i++)
                {
                    if (amount.Amount > rangesToApply[i].Minimum && amount.Amount > rangesToApply[i].Maximum)
                    {
                        taxValue += (rangesToApply[i].Maximum - rangesToApply[i].Minimum) * rangesToApply[i].Percent * 0.01M;
                    }
                    else if (amount.Amount > rangesToApply[i].Minimum && amount.Amount <= rangesToApply[i].Maximum)
                    {
                        taxValue += (amount.Amount - rangesToApply[i].Minimum) * rangesToApply[i].Percent * 0.01M;
                        break;
                    }
                }
            else
                taxValue = amount.Amount;

            var personalTax = info.Persons.FirstOrDefault(tp => tp.PersonId == personId && tp.GroupId == groupId);
            if (personalTax == null)
            {
                if (discount.HasValue && discount.Value.CurrencyCode != amount.CurrencyCode)
                    throw new UserFriendlyException(L("CurrencyMismatch"));

                return discount.HasValue
                    ? Money.Subtract(new Money(taxValue * info.Percent * 0.01M, amount.CurrencyCode), discount.Value)
                    : new Money(taxValue * info.Percent * 0.01M, amount.CurrencyCode);
            }

            switch (personalTax.MathType)
            {
                case MathType.Percent:
                    taxValue *= personalTax.Factor * 0.01M;
                    break;
                case MathType.MinimumWage:
                    taxValue = personalTax.Factor;
                    break;
                case MathType.Formula:
                    //var lexer = new FormulaLexer(personalTax.Formula);
                    //var tokens = lexer.Tokenize();
                    //var parser = new FormulaParser(tokens);
                    //var ast = parser.ParseExpression();
                    //var registry = new InstructionRegistry();
                    //var evaluator = new FormulaEvaluator(registry);
                    //var result = evaluator.Evaluate(ast);
                    //return new Money((decimal)result, amount.CurrencyCode);
                default:
                    throw new ArgumentOutOfRangeException();
            }
            
            if (discount.HasValue && discount.Value.CurrencyCode != amount.CurrencyCode)
                throw new UserFriendlyException(L("CurrencyMismatch"));

            return discount.HasValue
                ? Money.Subtract(new Money(taxValue * info.Percent * 0.01M, amount.CurrencyCode), discount.Value)
                : new Money(taxValue * info.Percent * 0.01M, amount.CurrencyCode);
        }
    }
}