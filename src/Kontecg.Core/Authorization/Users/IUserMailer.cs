using System.Threading.Tasks;

namespace Kontecg.Authorization.Users
{
    public interface IUserMailer
    {
        /// <summary>
        ///     Send email activation link to user's email address.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Email activation link</param>
        /// <param name="plainPassword">
        ///     Can be set to user's plain password to include it in the email.
        /// </param>
        Task SendEmailActivationLinkAsync(User user, string link = null, string plainPassword = null);

        /// <summary>
        ///     Sends a password reset link to user's email.
        /// </summary>
        /// <param name="user">User</param>
        /// <param name="link">Password reset link (optional)</param>
        Task SendPasswordResetLinkAsync(User user, string link = null);
    }
}
