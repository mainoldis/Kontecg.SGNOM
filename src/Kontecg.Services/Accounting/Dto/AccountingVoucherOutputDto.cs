using Kontecg.MultiCompany.Dto;
using System;
using System.Collections.Generic;
using Kontecg.Storage;

namespace Kontecg.Accounting.Dto
{
    public class AccountingVoucherOutputDto
    {
        public string Legal { get; set; }

        public CompanyInfoDto Company { get; set; }

        public int CompanyId { get; set; }

        public DocumentDefinitionDto DocumentDefinition { get; set; }

        public AccountingDocumentOutputDto Document { get; set; }

        public DateTime MadeOn { get; set; }

        public string Code { get; set; }

        public string Description { get; set; }

        public string Review { get; set; }

        public IReadOnlyList<AccountingVoucherNoteDto> Notes { get; set; }

        public TempFileInfo XmlFileExported { get; set; }
    }
}
