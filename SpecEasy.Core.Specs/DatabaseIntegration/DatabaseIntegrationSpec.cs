using System;
using System.Collections.Generic;
using System.Data.SqlServerCe;
using System.Linq;
using Dapper;
using NUnit.Framework;

namespace SpecEasy.Core.Specs.DatabaseIntegration
{
    public class DatabaseIntegrationSpec : Spec
    {
        private SqlCeConnection dbConnection;
        private SqlCeTransaction transaction;

        protected override void BeforeEachExample()
        {
            base.BeforeEachExample();
			dbConnection = new SqlCeConnection(DatabaseIntegrationSetup.TestDBConnectionString);
            dbConnection.Open();
            transaction = dbConnection.BeginTransaction();
        }

        protected override void AfterEachExample()
        {
            base.AfterEachExample();
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
                Then("the current context can find the single post created.", () => Assert.AreEqual(1, queryResult.Count)).
                Then("a subsequent assertion from within the same context should find the same single post, but it should not be duplicated.", () =>
                        Assert.IsTrue(queryResult.All(p => p.Body.Equals(firstPostBody)))));

            Given("a post is created in a context following but separate from a prevous context.", () => CreatePost(secondPostBody)).Verify(() =>
                Then("the current context should only find the single post created in this context.", () => Assert.AreEqual(1, queryResult.Count())));
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
