using NUnit.Framework;
using System;
using System.Data.SqlClient;

namespace console.Tests
{
    [TestFixture]
    public class ProgramTests
    {
        private const string ConnectionString = "User ID=sa;password=examlyMssql@123;server=localhost;trusted_connection=false;Persist Security Info=False;Encrypt=False;";
        private const string DatabaseName = "TestDatabase";
        private SqlConnection _setupConnection;
        private SqlConnection _testConnection;

        [SetUp]
        public void Setup()
        {
            // Create a connection to the SQL Server instance for setup
            _setupConnection = new SqlConnection(ConnectionString);
            _setupConnection.Open();

            // Drop the test database if it already exists
            DropDatabase(_setupConnection, DatabaseName);

            // Create the test database
            CreateDatabase(_setupConnection, DatabaseName);

            // Close the setup connection
            _setupConnection.Close();
            _setupConnection.Dispose();

            // Create a connection to the test database for the test method
            _testConnection = new SqlConnection($"{ConnectionString}Database={DatabaseName};");
            _testConnection.Open();

            // Create the Products table
            string createTableQuery = "CREATE TABLE Products (ID INT PRIMARY KEY, Name NVARCHAR(100), Rate DECIMAL(10, 2), Stock INT)";
            using (SqlCommand createTableCommand = new SqlCommand(createTableQuery, _testConnection))
            {
                createTableCommand.ExecuteNonQuery();
            }
        }

        [TearDown]
        public void Teardown()
        {
            // Close the test connection
            _testConnection.Close();
            _testConnection.Dispose();

            // Create a new connection to the SQL Server instance for teardown
            using (SqlConnection teardownConnection = new SqlConnection(ConnectionString))
            {
                teardownConnection.Open();

                // Drop the test database
                DropDatabase(teardownConnection, DatabaseName);

                // Close the teardown connection
                teardownConnection.Close();
            }
        }

        [Test]
        public void AddProductToTable_ProductAddedSuccessfully()
        {
            // Arrange
            // Use the test connection to perform operations
            using (SqlCommand insertCommand = new SqlCommand("INSERT INTO Products (ID, Name, Rate, Stock) VALUES (1, 'Example Product', 9.99, 100)", _testConnection))
            {
                // Act
                int rowsAffected = insertCommand.ExecuteNonQuery();

                // Assert
                Assert.AreEqual(1, rowsAffected);
            }
        }

        [Test]
        public void ConnectToDatabase_ConnectionSuccessful()
        {
            // Arrange
            bool connectionSuccess = false;
            string errorMessage = "";

            // Act
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    connectionSuccess = true;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
            }

            // Assert
            Assert.IsTrue(connectionSuccess, "Failed to connect to the database. Error message: " + errorMessage);
        }

        private void CreateDatabase(SqlConnection connection, string databaseName)
        {
            string createDatabaseQuery = $"CREATE DATABASE {databaseName}";
            using (SqlCommand createDatabaseCommand = new SqlCommand(createDatabaseQuery, connection))
            {
                createDatabaseCommand.ExecuteNonQuery();
            }
        }

        private void DropDatabase(SqlConnection connection, string databaseName)
        {
            string dropDatabaseQuery = $"IF EXISTS (SELECT 1 FROM sys.databases WHERE name = '{databaseName}') BEGIN ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{databaseName}] END";
            using (SqlCommand dropDatabaseCommand = new SqlCommand(dropDatabaseQuery, connection))
            {
                dropDatabaseCommand.ExecuteNonQuery();
            }
        }
    }
}
