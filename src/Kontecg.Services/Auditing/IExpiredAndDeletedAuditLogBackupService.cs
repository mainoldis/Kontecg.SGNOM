using System.Collections.Generic;

namespace Kontecg.Auditing
{
    public interface IExpiredAndDeletedAuditLogBackupService
    {
        bool CanBackup();

        void Backup(List<AuditLog> auditLogs);
    }
}
