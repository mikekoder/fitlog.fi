using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit.Training
{
    public class TrainingRepository : RepositoryBase
    {
        public TrainingRepository(DbProviderFactory dbFactory, string connectionString) : base(dbFactory, connectionString)
        {
        }


    }
}
