using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using Dapper;
using Should;

namespace SpecEasy.Specs.DatabaseIntegration
{
    public class DatabaseIntegrationSpec : Spec
    {
        private SqlCeConnection dbConnection;
        private SqlCeTransaction transaction;

        protected override void BeforeEachExample()
        {
			dbConnection = new SqlCeConnection(DatabaseIntegrationSetup.TestDbConnectionString);
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
