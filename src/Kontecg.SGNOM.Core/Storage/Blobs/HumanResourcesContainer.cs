using Kontecg.BlobStoring;

namespace Kontecg.Storage.Blobs
{
    [BlobContainerName("human-resources")]
    public class HumanResourcesContainer
    {
        public const string Documents = "documents";
        public const string Calendars = "calendars";
    }
}
