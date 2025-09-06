using System;

namespace Kontecg.Authorization.Users.Dto
{
    public class ImportUsersFromExcelJobArgs
    {
        public int? CompanyId { get; set; }

        public Guid BinaryObjectId { get; set; }

        public UserIdentifier User { get; set; }
    }
}
