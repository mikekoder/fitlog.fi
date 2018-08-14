using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crash.Fit
{
    public abstract class RepositoryBase
    {
        private readonly string connectionString;
        public RepositoryBase(string connectionString)
        {
            this.connectionString = connectionString;
        }
        protected SqlConnection CreateConnection()
        {
            var connection = new SqlConnection(connectionString);
            connection.Open();
            return connection;
        }
    }
}
