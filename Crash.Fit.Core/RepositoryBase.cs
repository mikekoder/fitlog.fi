using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit
{
    public abstract class RepositoryBase
    {
        private readonly DbProviderFactory factory;
        private readonly string connectionString;
        public RepositoryBase(DbProviderFactory dbFactory, string connectionString)
        {
            this.factory = dbFactory;
            this.connectionString = connectionString;
        }
        protected IDbConnection CreateConnection()
        {
            var connection = factory.CreateConnection();
            connection.ConnectionString = connectionString;
            connection.Open();
            return connection;
        }
    }
}
