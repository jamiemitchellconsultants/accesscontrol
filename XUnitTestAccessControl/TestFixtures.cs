using System;
using System.Collections.Generic;
using System.Text;
using AccessControl.Models;
using AccessControl.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using Xunit;

namespace XUnitTestAccessControl
{

    public class TestFixtures
    {
        public DatabaseFixture DatabaseFixture { get; set; }
        public PermissionCheckFixture PermissionCheckFixture { get; set; }

        public TestFixtures()
        {
            DatabaseFixture=new DatabaseFixture();
            PermissionCheckFixture=new PermissionCheckFixture(DatabaseFixture);
        }

    }
    public class DatabaseFixture
    {
        public AccessControlContext ACContext { get; set; }
        public DatabaseFixture()
        {
            var connection = new SqliteConnection("Datasource=:memory:");
            connection.Open();
            var options = new DbContextOptionsBuilder<AccessControlContext>().UseSqlite(connection)
                .Options;
            ACContext = new AccessControlContext(options);
            ACContext.Database.EnsureCreated();
        }

        

    }

    public class PermissionCheckFixture:IClassFixture<DatabaseFixture>
    {
        public PermissionCheck PermissionCheck { get; set; }
        public PermissionCheckFixture(DatabaseFixture dbDatabaseFixture)
        {
            PermissionCheck=new PermissionCheck(dbDatabaseFixture.ACContext);


        }
    }
}
