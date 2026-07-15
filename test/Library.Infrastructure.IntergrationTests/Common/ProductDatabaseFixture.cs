using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Infrastructure.IntergrationTests.Common
{
    public sealed class ProductDatabaseFixture : MySqlFixture
    {
        public ProductDatabaseFixture() : base("Server=localhost;Port=13306;Database=ProductDB;Uid=root;Pwd=MyStrongPass123!;")
        {
        }
    }
}
