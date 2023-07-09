using System;
using System.Data.SqlClient;

namespace console
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "User ID=sa;password=examlyMssql@123;server=localhost;Database=demo;trusted_connection=false;Persist Security Info=False;Encrypt=False;";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Connected to the database.");

                    // Add product to the product table
                    string insertQuery = "INSERT INTO Products (ID, Name, Rate, Stock) VALUES (2, 'Example Product', 9.99, 100)";
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        int rowsAffected = command.ExecuteNonQuery();
                        Console.WriteLine($"{rowsAffected} row(s) affected.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error connecting to the database:");
                    Console.WriteLine(ex.Message);
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
