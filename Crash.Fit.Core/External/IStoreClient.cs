using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Crash.Fit.External
{
    public interface IStoreClient
    {
        Task<ExternalFood> FindFood(string ean);
    }
}
