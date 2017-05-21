using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.Profile
{
    public interface IProfileRepository
    {
        Profile GetProfile(Guid userId);
        bool SaveProfile(Profile profile);
    }
}
