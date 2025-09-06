using Kontecg.BlobStoring;

namespace Kontecg.Storage.Blobs
{
    [BlobContainerName("accounting")]
    public class AccountingContainer
    {
        public const string Documents = "documents";
        public const string Vouchers = "vouchers";
    }
}