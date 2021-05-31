using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fitlog.External
{
    public interface IStoreClient
    {
        Task<ExternalFood> FindFood(string ean);
    }
}
