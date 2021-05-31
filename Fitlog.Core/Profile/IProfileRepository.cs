using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.Profile
{
    public interface IProfileRepository
    {
        Profile GetProfile(Guid userId);
        void SaveProfile(Profile profile);
        string GetRefreshToken(Guid userId);
        string UpdateRefreshToken(Guid userId);
        Guid? GetUserIdByRefreshToken(string refreshToken);
    }
}
