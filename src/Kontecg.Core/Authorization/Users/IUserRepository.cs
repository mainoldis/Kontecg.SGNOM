using System;
using System.Collections.Generic;
using Kontecg.Domain.Repositories;

namespace Kontecg.Authorization.Users
{
    public interface IUserRepository : IRepository<User, long>
    {
        List<long> GetPasswordExpiredUserIds(DateTime passwordExpireDate);

        void UpdateUsersToChangePasswordOnNextLogin(List<long> userIdsToUpdate);
    }
}
