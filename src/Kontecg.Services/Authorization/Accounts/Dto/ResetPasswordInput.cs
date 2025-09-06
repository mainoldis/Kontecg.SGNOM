using System;
using System.Web;
using Kontecg.Auditing;
using Kontecg.Runtime.Security;
using Kontecg.Runtime.Validation;

namespace Kontecg.Authorization.Accounts.Dto
{
    public class ResetPasswordInput : IShouldNormalize
    {
        public long UserId { get; set; }

        public string ResetCode { get; set; }

        public DateTime ExpireDate { get; set; }

        [DisableAuditing] public string Password { get; set; }

        public string ReturnUrl { get; set; }

        public string SingleSignIn { get; set; }

        /// <summary>
        ///     Encrypted values for {CompanyId}, {UserId} and {ResetCode}
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
                try
                {
                    var parameters = SimpleStringCipher.Instance.Decrypt(C);
                    var query = HttpUtility.ParseQueryString(parameters);

                    if (query["userId"] != null) UserId = Convert.ToInt32(query["userId"]);

                    if (query["resetCode"] != null) ResetCode = query["resetCode"];

                    if (query["expireDate"] == null) throw new KontecgValidationException();

                    ExpireDate = Convert.ToDateTime(query["expireDate"]);
                }
                catch (Exception ex)
                {
                    throw new KontecgValidationException("Invalid reset password link!", ex);
                }
            }
        }
    }
}
