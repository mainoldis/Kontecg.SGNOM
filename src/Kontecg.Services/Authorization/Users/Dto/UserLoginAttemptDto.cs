using System;

namespace Kontecg.Authorization.Users.Dto
{
    public class UserLoginAttemptDto
    {
        public string CompanyName { get; set; }

        public string UserNameOrEmail { get; set; }

        public string ClientIpAddress { get; set; }

        public string ClientName { get; set; }

        public string ClientInfo { get; set; }

        public string Result { get; set; }

        public DateTime CreationTime { get; set; }
    }
}
