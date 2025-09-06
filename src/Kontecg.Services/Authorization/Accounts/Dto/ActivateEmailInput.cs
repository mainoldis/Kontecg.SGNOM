using System;
using System.Web;
using Kontecg.Runtime.Security;
using Kontecg.Runtime.Validation;

namespace Kontecg.Authorization.Accounts.Dto
{
    public class ActivateEmailInput : IShouldNormalize
    {
        public long UserId { get; set; }

        public string ConfirmationCode { get; set; }

        /// <summary>
        ///     Encrypted values for {CompanyId}, {UserId} and {ConfirmationCode}
        /// </summary>
        public string C { get; set; }

        public void Normalize()
        {
            ResolveParameters();
        }

        protected virtual void ResolveParameters()
        {
            if (!string.IsNullOrEmpty(C))
            {
                var parameters = SimpleStringCipher.Instance.Decrypt(C);
                var query = HttpUtility.ParseQueryString(parameters);

                if (query["userId"] != null) UserId = Convert.ToInt32(query["userId"]);

                if (query["confirmationCode"] != null) ConfirmationCode = query["confirmationCode"];
            }
        }
    }
}
