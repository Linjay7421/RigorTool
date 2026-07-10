using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Application.IntegrationTests.Common
{
    public sealed class ProductDatabaseFixture : MySqlFixture
    {
        public ProductDatabaseFixture() : base("Server=localhost;Port=23306;Database=FileDB;Uid=root;Pwd=MyStrongPass123!;")
        {
        }
    }
}
