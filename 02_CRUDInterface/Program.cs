using System;
using System.Configuration;
using System.Data.SqlClient;

namespace _02_CRUDInterface
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["SportShopDB"].ConnectionString;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                Console.WriteLine("Connected successfully!");

                while (true)
                {
                    Console.WriteLine("\nChoose an option:");
                    Console.WriteLine("1. Add new sale");
                    Console.WriteLine("2. Show sales for a certain period");
                    Console.WriteLine("3. Show the last purchase of a client");
                    Console.WriteLine("4. Delete a seller or client by ID");
                    Console.WriteLine("5. Show the seller with the highest total sales");
                    Console.WriteLine("6. Exit");
                    int choice = int.Parse(Console.ReadLine()!);

                    switch (choice)
                    {
                        case 1:
                            AddNewSale(sqlConnection);
                            break;
                        case 2:
                            ShowSalesForPeriod(sqlConnection);
                            break;
                        case 3:
                            ShowLastPurchaseByClient(sqlConnection);
                            break;
                        case 4:
                            DeleteById(sqlConnection);
                            break;
                        case 5:
                            ShowTopSeller(sqlConnection);
                            break;
                        case 6:
                            return;
                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }
                }
            }
        }

        static void AddNewSale(SqlConnection sqlConnection)
        {
            Console.WriteLine("Enter Product ID:");
            int productId = int.Parse(Console.ReadLine()!);
            Console.WriteLine("Enter Price:");
            decimal price = decimal.Parse(Console.ReadLine()!);
            Console.WriteLine("Enter Quantity:");
            int quantity = int.Parse(Console.ReadLine()!);
            Console.WriteLine("Enter Employee ID:");
            int employeeId = int.Parse(Console.ReadLine()!);
            Console.WriteLine("Enter Client ID:");
            int clientId = int.Parse(Console.ReadLine()!);

            string insertQuery = @"INSERT INTO Salles (ProductId, Price, Quantity, EmployeeId, ClientId) 
                                   VALUES (@ProductId, @Price, @Quantity, @EmployeeId, @ClientId)";

            using (SqlCommand command = new SqlCommand(insertQuery, sqlConnection))
            {
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@EmployeeId", employeeId);
                command.Parameters.AddWithValue("@ClientId", clientId);
                command.ExecuteNonQuery();
            }
            Console.WriteLine("New sale added successfully!");
        }

        static void ShowSalesForPeriod(SqlConnection sqlConnection)
        {
            Console.WriteLine("Enter start date (yyyy-mm-dd):");
            DateTime startDate = DateTime.Parse(Console.ReadLine()!);
            Console.WriteLine("Enter end date (yyyy-mm-dd):");
            DateTime endDate = DateTime.Parse(Console.ReadLine()!);

            string query = @"SELECT Id, ProductId, Price, Quantity, SaleDate FROM Salles 
                             WHERE SaleDate BETWEEN @StartDate AND @EndDate";

            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                command.Parameters.AddWithValue("@StartDate", startDate);
                command.Parameters.AddWithValue("@EndDate", endDate);
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Console.WriteLine($"Sale ID: {reader["Id"]}, Product ID: {reader["ProductId"]}, Price: {reader["Price"]}, Quantity: {reader["Quantity"]}, Date: {reader["SaleDate"]}");
                }
                reader.Close();
            }
        }

        static void ShowLastPurchaseByClient(SqlConnection sqlConnection)
        {
            Console.WriteLine("Enter client full name:");
            string fullName = Console.ReadLine()!;

            string query = @"SELECT TOP 1 S.Id, S.ProductId, S.Price, S.Quantity, S.SaleDate
                             FROM Salles AS S
                             INNER JOIN Clients AS C ON S.ClientId = C.Id
                             WHERE C.FullName = @FullName
                             ORDER BY S.SaleDate DESC";

            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                command.Parameters.AddWithValue("@FullName", fullName);
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine($"Last purchase - Sale ID: {reader["Id"]}, Product ID: {reader["ProductId"]}, Price: {reader["Price"]}, Quantity: {reader["Quantity"]}, Date: {reader["SaleDate"]}");
                }
                else
                {
                    Console.WriteLine("No purchases found for this client.");
                }
                reader.Close();
            }
        }

        static void DeleteById(SqlConnection sqlConnection)
        {
            Console.WriteLine("Enter ID to delete:");
            int id = int.Parse(Console.ReadLine()!);
            Console.WriteLine("Delete from (1) Employees or (2) Clients?");
            int tableChoice = int.Parse(Console.ReadLine()!);

            if (tableChoice == 1)
            {
                string deleteSalesQuery = "DELETE FROM Salles WHERE EmployeeId = @Id";
                using (SqlCommand command = new SqlCommand(deleteSalesQuery, sqlConnection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                string deleteQuery = "DELETE FROM Employees WHERE Id = @Id";
                using (SqlCommand command = new SqlCommand(deleteQuery, sqlConnection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("Record deleted successfully!");
                    }
                    else
                    {
                        Console.WriteLine("No record found with this ID.");
                    }
                }
            }
        }

        static void ShowTopSeller(SqlConnection sqlConnection)
        {
            string query = @"SELECT TOP 1 E.FullName, SUM(S.Price * S.Quantity) AS TotalSales
                             FROM Salles AS S
                             INNER JOIN Employees AS E ON S.EmployeeId = E.Id
                             GROUP BY E.FullName
                             ORDER BY TotalSales DESC";

            using (SqlCommand command = new SqlCommand(query, sqlConnection))
            {
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    Console.WriteLine($"Top seller: {reader["FullName"]} with total sales of {reader["TotalSales"]}");
                }
                reader.Close();
            }
        }
    }
}