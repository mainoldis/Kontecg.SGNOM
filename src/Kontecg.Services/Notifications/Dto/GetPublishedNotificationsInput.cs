using System;

namespace Kontecg.Notifications.Dto
{
    public class GetPublishedNotificationsInput
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }
    }
}
