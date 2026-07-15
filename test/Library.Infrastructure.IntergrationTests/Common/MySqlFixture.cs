using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Web.Library.Infrastructure.Persistence;

namespace Library.Infrastructure.IntergrationTests.Common
{
    public abstract class MySqlFixture : IDisposable
    {
        public string ConnectionString { get; }

        public MySqlConnectionFactory ConnectionFactory { get; }

        protected MySqlFixture(string connectionString)
        {
            ConnectionString = connectionString;
                
            ConnectionFactory =
                new MySqlConnectionFactory(ConnectionString);
        }

        public virtual void Dispose()
        {
            // Disposed test mutations.
        }
    }
}
