using System.Data.SqlClient;

namespace _01_ConnectedMode
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = @"Data Source=DESKTOP-9U0F2B0\SQLEXPRESSS;
                                      Initial Catalog = SportShop;
                                      Integrated Security=True;
                                      Connect Timeout=30;
                                      Encrypt=False;";
            SqlConnection sqlConnection = new SqlConnection(connectionString);

            sqlConnection.Open();
            Console.WriteLine("Connected sucessfuly");



            // 1.Відображення всіх клієнтів
            string queryClients = "SELECT Id, FullName, Email, Phone, Gender, PercentSale, Subscribe FROM Clients";
            SqlCommand commandClients = new SqlCommand(queryClients, sqlConnection);
            SqlDataReader readerClients = commandClients.ExecuteReader();
            Console.WriteLine();
            Console.WriteLine("List of clients:");
            while (readerClients.Read())
            {
                int id = readerClients.GetInt32(0);
                string fullName = readerClients.GetString(1);
                string email = readerClients.GetString(2);
                string phone = readerClients.GetString(3);
                string gender = readerClients.GetString(4);
                int percentSale = readerClients.GetInt32(5);
                bool subscribe = readerClients.GetBoolean(6);

                Console.WriteLine($"ID: {id}, Name: {fullName}, Email: {email}, Phone: {phone}, Gender: {gender}, Percent Sale: {percentSale}, Subscribed: {subscribe}");
            }
            readerClients.Close();


            // 2.Відображення всіх продавців
            string queryEmployees = "SELECT Id, FullName, HireDate, Gender, Salary FROM Employees";
            SqlCommand commandEmployees = new SqlCommand(queryEmployees, sqlConnection);
            SqlDataReader readerEmployees = commandEmployees.ExecuteReader();
            Console.WriteLine();
            Console.WriteLine("List of employees:");
            while (readerEmployees.Read())
            {
                int id = readerEmployees.GetInt32(0);
                string fullName = readerEmployees.GetString(1);
                DateTime hireDate = readerEmployees.GetDateTime(2);
                string gender = readerEmployees.GetString(3);
                decimal salary = readerEmployees.GetDecimal(4);

                Console.WriteLine($"ID: {id}, Name: {fullName}, Hire Date: {hireDate.ToShortDateString()}, Gender: {gender}, Salary: {salary}");
            }
            readerEmployees.Close();


            // Відображення всіх товарів
            string queryProducts = "SELECT Id, Name, TypeProduct, Quantity, CostPrice, Producer, Price FROM Products";
            SqlCommand commandProducts = new SqlCommand(queryProducts, sqlConnection);
            SqlDataReader readerProducts = commandProducts.ExecuteReader();
            Console.WriteLine();
            Console.WriteLine("List of products:");
            while (readerProducts.Read())
            {
                int id = readerProducts.GetInt32(0);
                string name = readerProducts.GetString(1);
                string typeProduct = readerProducts.GetString(2);
                int quantity = readerProducts.GetInt32(3);
                int costPrice = readerProducts.GetInt32(4); 
                string producer = readerProducts.IsDBNull(5) ? "N/A" : readerProducts.GetString(5);
                int price = readerProducts.GetInt32(6); 

                Console.WriteLine($"ID: {id}, Name: {name}, Type: {typeProduct}, Quantity: {quantity}, Cost Price: {costPrice}, Producer: {producer}, Price: {price}");
            }
            readerProducts.Close();


            // 3.Відображення продаж певного продавця
            string employeeFullName = "Yaroshchuk Ivan Petrovych"; // Ім'я та прізвище продавця
            string querySalesByEmployee = @"SELECT S.Id, S.ProductId, S.Price, S.Quantity, S.EmployeeId, S.ClientId 
                                            FROM Salles AS S
                                            INNER JOIN Employees AS E ON S.EmployeeId = E.Id
                                            WHERE E.FullName = @FullName";

            SqlCommand commandSalesByEmployee = new SqlCommand(querySalesByEmployee, sqlConnection);
            commandSalesByEmployee.Parameters.AddWithValue("@FullName", employeeFullName);
            SqlDataReader readerSalesByEmployee = commandSalesByEmployee.ExecuteReader();
            Console.WriteLine();
            Console.WriteLine($"Sales by employee: {employeeFullName}");
            while (readerSalesByEmployee.Read())
            {
                int saleId = readerSalesByEmployee.GetInt32(0);
                int productId = readerSalesByEmployee.GetInt32(1);
                decimal price = readerSalesByEmployee.GetDecimal(2);
                int quantity = readerSalesByEmployee.GetInt32(3);
                int employeeId = readerSalesByEmployee.GetInt32(4);
                int clientId = readerSalesByEmployee.GetInt32(5);

                Console.WriteLine($"Sale ID: {saleId}, Product ID: {productId}, Price: {price}, Quantity: {quantity}, Employee ID: {employeeId}, Client ID: {clientId}");
            }
            readerSalesByEmployee.Close();


            // 4.Відображення продаж на суму більше певної
            decimal amount = 500.0m; // Сума, більше якої необхідно відображати продажі
            string querySalesAboveAmount = "SELECT Id, ProductId, Price, Quantity FROM Salles WHERE Price * Quantity > @Amount";
            SqlCommand commandSalesAboveAmount = new SqlCommand(querySalesAboveAmount, sqlConnection);
            commandSalesAboveAmount.Parameters.AddWithValue("@Amount", amount);
            SqlDataReader readerSalesAboveAmount = commandSalesAboveAmount.ExecuteReader();
            Console.WriteLine();
            Console.WriteLine($"Sales with total amount greater than {amount}:");
            while (readerSalesAboveAmount.Read())
            {
                int saleId = readerSalesAboveAmount.GetInt32(0);
                int productId = readerSalesAboveAmount.GetInt32(1);
                decimal price = readerSalesAboveAmount.GetDecimal(2);
                int quantity = readerSalesAboveAmount.GetInt32(3);

                Console.WriteLine($"Sale ID: {saleId}, Product ID: {productId}, Price: {price}, Quantity: {quantity}");
            }
            readerSalesAboveAmount.Close();


            //5. Відображення найдорожчої та найдешевшої покупки певного клієнта
            string clientFullName = "Petruk Stepan Romanovych"; // Ім'я та прізвище клієнта
            // Найдорожча покупка
            string queryMaxPurchase = @"SELECT TOP 1 S.Id, S.ProductId, S.Price, S.Quantity
                                        FROM Salles AS S
                                        INNER JOIN Clients AS C ON S.ClientId = C.Id
                                        WHERE C.FullName = @FullName
                                        ORDER BY S.Price * S.Quantity DESC";
            SqlCommand commandMaxPurchase = new SqlCommand(queryMaxPurchase, sqlConnection);
            commandMaxPurchase.Parameters.AddWithValue("@FullName", clientFullName);
            SqlDataReader readerMaxPurchase = commandMaxPurchase.ExecuteReader();

            if (readerMaxPurchase.Read())
            {
                Console.WriteLine();
                Console.WriteLine("Most expensive purchase:");
                int saleId = readerMaxPurchase.GetInt32(0);
                int productId = readerMaxPurchase.GetInt32(1);
                decimal price = readerMaxPurchase.GetDecimal(2);
                int quantity = readerMaxPurchase.GetInt32(3);

                Console.WriteLine($"Sale ID: {saleId}, Product ID: {productId}, Price: {price}, Quantity: {quantity}");
            }
            readerMaxPurchase.Close();


            // Найдешевша покупка
            string queryMinPurchase = @"SELECT TOP 1 S.Id, S.ProductId, S.Price, S.Quantity
                                        FROM Salles AS S
                                        INNER JOIN Clients AS C ON S.ClientId = C.Id
                                        WHERE C.FullName = @FullName
                                        ORDER BY S.Price * S.Quantity ASC";
            SqlCommand commandMinPurchase = new SqlCommand(queryMinPurchase, sqlConnection);
            commandMinPurchase.Parameters.AddWithValue("@FullName", clientFullName);
            SqlDataReader readerMinPurchase = commandMinPurchase.ExecuteReader();

            if (readerMinPurchase.Read())
            {
                Console.WriteLine("Cheapest purchase:");
                int saleId = readerMinPurchase.GetInt32(0);
                int productId = readerMinPurchase.GetInt32(1);
                decimal price = readerMinPurchase.GetDecimal(2);
                int quantity = readerMinPurchase.GetInt32(3);

                Console.WriteLine($"Sale ID: {saleId}, Product ID: {productId}, Price: {price}, Quantity: {quantity}");
            }
            readerMinPurchase.Close();


            // 6.Відображення найпершої продажі певного продавця
            string queryFirstSale = @"SELECT TOP 1 S.Id, S.ProductId, S.Price, S.Quantity 
                                      FROM Salles AS S
                                      INNER JOIN Employees AS E ON S.EmployeeId = E.Id
                                      WHERE E.FullName = @FullName
                                      ORDER BY S.Id ASC";

            SqlCommand commandFirstSale = new SqlCommand(queryFirstSale, sqlConnection);
            commandFirstSale.Parameters.AddWithValue("@FullName", employeeFullName);
            SqlDataReader readerFirstSale = commandFirstSale.ExecuteReader();

            if (readerFirstSale.Read())
            {
                int saleId = readerFirstSale.GetInt32(0);
                int productId = readerFirstSale.GetInt32(1);
                decimal price = readerFirstSale.GetDecimal(2);
                int quantity = readerFirstSale.GetInt32(3);

                Console.WriteLine($"First sale of {employeeFullName}:");
                Console.WriteLine($"Sale ID: {saleId}, Product ID: {productId}, Price: {price}, Quantity: {quantity}");
            }
            readerFirstSale.Close();














            sqlConnection.Close();
        }
    }
}




