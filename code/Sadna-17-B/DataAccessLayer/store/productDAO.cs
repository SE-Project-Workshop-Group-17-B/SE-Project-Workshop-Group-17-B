using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;

namespace Sadna_17_B.DataAccessLayer.store
{
    public class ProductDAO
    {
        private string connectionString;

        public ProductDAO()
        {
            string connectionName = OrmLiteHelper.memoryDB ? "SQLiteDB-Memory" : "SQLiteDB";
            connectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
            Console.WriteLine("Connection String: " + connectionString);
        }

        public void CreateProductTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Database connection opened.");

                    string query = @"
                    CREATE TABLE IF NOT EXISTS Product (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        StoreID INTEGER NOT NULL,
                        Amount INTEGER NOT NULL,
                        Name TEXT NOT NULL,
                        Price REAL NOT NULL,
                        Rating REAL NOT NULL,
                        Category TEXT NOT NULL,
                        Description TEXT,
                        Reviews TEXT,
                        Locked INTEGER NOT NULL,
                        FOREIGN KEY (StoreID) REFERENCES Store(ID)
                    );";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Product table created.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the Product table: " + ex.Message);
            }
        }

        public void AddProduct(ProductDTO product)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                INSERT INTO Product (StoreID, Amount, Name, Price, Rating, Category, Description, Reviews, Locked) 
                VALUES (@StoreID, @Amount, @Name, @Price, @Rating, @Category, @Description, @Reviews, @Locked);";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StoreID", product.StoreID);
                    command.Parameters.AddWithValue("@Amount", product.Amount);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Rating", product.Rating);
                    command.Parameters.AddWithValue("@Category", product.Category);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@Reviews", string.Join(";", product.Reviews));
                    command.Parameters.AddWithValue("@Locked", product.Locked ? 1 : 0);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Product added to the table.");
                }
            }
        }

        public void UpdateProduct(ProductDTO product)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                UPDATE Product 
                SET StoreID = @StoreID, 
                    Amount = @Amount, 
                    Name = @Name, 
                    Price = @Price, 
                    Rating = @Rating, 
                    Category = @Category, 
                    Description = @Description, 
                    Reviews = @Reviews, 
                    Locked = @Locked 
                WHERE ID = @ID;";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StoreID", product.StoreID);
                    command.Parameters.AddWithValue("@Amount", product.Amount);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Rating", product.Rating);
                    command.Parameters.AddWithValue("@Category", product.Category);
                    command.Parameters.AddWithValue("@Description", product.Description);
                    command.Parameters.AddWithValue("@Reviews", string.Join(";", product.Reviews));
                    command.Parameters.AddWithValue("@Locked", product.Locked ? 1 : 0);
                    command.Parameters.AddWithValue("@ID", product.ID);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Product updated.");
                }
            }
        }

        public void DeleteProduct(int productId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Product WHERE ID = @ID;";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", productId);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Product deleted.");
                }
            }
        }

        public ProductDTO GetProduct(int productId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Product WHERE ID = @ID;";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", productId);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine("Product retrieved.");
                            return new ProductDTO
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                StoreID = Convert.ToInt32(reader["StoreID"]),
                                Amount = Convert.ToInt32(reader["Amount"]),
                                Name = reader["Name"].ToString(),
                                Price = Convert.ToDouble(reader["Price"]),
                                Rating = Convert.ToDouble(reader["Rating"]),
                                Category = reader["Category"].ToString(),
                                Description = reader["Description"].ToString(),
                                Reviews = reader["Reviews"].ToString(),
                                Locked = Convert.ToBoolean(reader["Locked"])
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
