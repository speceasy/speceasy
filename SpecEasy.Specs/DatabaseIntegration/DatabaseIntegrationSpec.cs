using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.IO;
using System.Linq;
using Dapper;
using NUnit.Framework;
using Should;

namespace SpecEasy.Specs.DatabaseIntegration
{
    public class DatabaseIntegrationSpec : Spec
    {
        private static readonly string DatabaseFileName = $"{Guid.NewGuid().ToString("N").Substring(0, 8).ToLowerInvariant()}.sdf";

        private static string TestDbConnectionString => $"Data Source={DatabaseFileName};";

        private SqlCeEngine sqlCeEngine;
        private SqlCeConnection dbConnection;
        private SqlCeTransaction transaction;

        [OneTimeSetUp]
        public void BeforeDatabaseIntegration()
        {
            if (File.Exists(DatabaseFileName))
                File.Delete(DatabaseFileName);

            sqlCeEngine = new SqlCeEngine(TestDbConnectionString);
            sqlCeEngine.CreateDatabase();

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

        [OneTimeTearDown]
        public void AfterDatabaseIntegration()
        {
            try
            {
                sqlCeEngine.Dispose();
                File.Delete(DatabaseFileName);
            }
            catch (IOException)
            {
            }
        }

        protected override void BeforeEachExample()
        {
            dbConnection = new SqlCeConnection(TestDbConnectionString);
            dbConnection.Open();
            transaction = dbConnection.BeginTransaction();
        }

        protected override void AfterEachExample()
        {
            transaction.Rollback();
            transaction.Dispose();
            dbConnection.Close();
            dbConnection.Dispose();
        }

        public void DataRolledBackBetweenLogicalTests()
        {
            IList<dynamic> queryResult = null;

            const string firstPostBody = "this is the body of the first post.";
            const string secondPostBody = "body of the second post.";

            When("querying a database within a spec", () => queryResult = GetPosts());

            Given("a post is created in a test context.", () => CreatePost(firstPostBody)).Verify(() =>
                Then("the current context can find the single post created.", () => queryResult.Count.ShouldEqual(1)).
                Then("a subsequent assertion from within the same context should find the same single post, but it should not be duplicated.", () =>
                        queryResult.All(p => p.Body.Equals(firstPostBody)).ShouldBeTrue()));

            Given("a post is created in a context following but separate from a previous context.", () => CreatePost(secondPostBody)).Verify(() =>
                Then("the current context should only find the single post created in this context.", () => queryResult.Count.ShouldEqual(1)));
        }

        private IList<dynamic> GetPosts()
        {
            return dbConnection.Query("select Id, CreateDate, Author, Body from Posts", transaction: transaction).ToList();
        }

        private void CreatePost(string body)
        {
            dbConnection.Execute("insert Posts (CreateDate, Author, Body) values (@CreateDate, @Author, @Body)", new
            {
                CreateDate = DateTime.UtcNow,
                Author = "Spec Easy",
                Body = body
            }, transaction: transaction);
        }
    }
}
