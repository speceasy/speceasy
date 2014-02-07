using System.Data.SqlServerCe;
using System.IO;
using Dapper;
using NUnit.Framework;

namespace SpecEasy.Specs.DatabaseIntegration
{

    [SetUpFixture]
    public class DatabaseIntegrationSetup
    {
        public const string TestDbConnectionString = "Data Source=TestDB.sdf;";

        [SetUp]
        public void BeforeDatabaseIntegration()
        {
            if (File.Exists("TestDB.sdf"))
                File.Delete("TestDB.sdf");

            var engine = new SqlCeEngine(TestDbConnectionString);
            engine.CreateDatabase();

            using (var connection = new SqlCeConnection(TestDbConnectionString))
            {
                connection.Execute(@"
                   create table Posts (
                      Id INT Primary Key Identity(1,1),
                      CreateDate DATETIME NOT NULL,
                      Author nvarchar(100),
                      Body nvarchar(4000)
                    )");
            }
        }
    }
}
