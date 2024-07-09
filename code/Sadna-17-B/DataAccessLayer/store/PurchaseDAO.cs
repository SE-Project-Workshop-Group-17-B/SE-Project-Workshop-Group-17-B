using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DataAccessLayer.store
{
    public class PurchaseDAO
    {
        private readonly string connectionString;

        public PurchaseDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        }

        public void CreatePurchaseTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Database connection opened.");

                    string query = @"
                    CREATE TABLE IF NOT EXISTS Purchase (
                        Id INTEGER PRIMARY KEY,
                        Name TEXT NOT NULL,
                        AggregationRule TEXT NOT NULL,
                        Conditions TEXT NOT NULL
                    );";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Purchase table created.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the Purchase table: " + ex.Message);
            }
        }

        public void AddPurchase(PurchaseDTO purchase)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "INSERT INTO Purchase (Name, AggregationRule, Conditions) VALUES (@Name, @AggregationRule, @Conditions);";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Name", purchase.Name);
                        command.Parameters.AddWithValue("@AggregationRule", purchase.AggregationRule);
                        command.Parameters.AddWithValue("@Conditions", purchase.Conditions);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while adding a purchase: " + ex.Message);
            }
        }

        public PurchaseDTO GetPurchase(int id)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Purchase WHERE Id = @Id;";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new PurchaseDTO
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = reader["Name"].ToString(),
                                    AggregationRule = reader["AggregationRule"].ToString(),
                                    Conditions = reader["Conditions"].ToString()
                                };
                            }
                        }
                    }
                    connection.Close();

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving a purchase: " + ex.Message);
            }
            return null;
        }

        public List<PurchaseDTO> GetAllPurchases()
        {
            List<PurchaseDTO> purchases = new List<PurchaseDTO>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Purchase;";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                purchases.Add(new PurchaseDTO
                                {
                                    Id = Convert.ToInt32(reader["Id"]),
                                    Name = reader["Name"].ToString(),
                                    AggregationRule = reader["AggregationRule"].ToString(),
                                    Conditions = reader["Conditions"].ToString()
                                });
                            }
                        }
                    }
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving all purchases: " + ex.Message);
            }
            return purchases;
        }

        public void UpdatePurchase(PurchaseDTO purchase)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "UPDATE Purchase SET Name = @Name, AggregationRule = @AggregationRule, Conditions = @Conditions WHERE Id = @Id;";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", purchase.Id);
                        command.Parameters.AddWithValue("@Name", purchase.Name);
                        command.Parameters.AddWithValue("@AggregationRule", purchase.AggregationRule);
                        command.Parameters.AddWithValue("@Conditions", purchase.Conditions);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while updating a purchase: " + ex.Message);
            }
        }

        public void DeletePurchase(int id)
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "DELETE FROM Purchase WHERE Id = @Id;";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while deleting a purchase: " + ex.Message);
            }
        }
    }
}