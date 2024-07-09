using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SQLite;
using System.Linq;

namespace Sadna_17_B.DataAccessLayer.store
{
    public class StoreDAO
    {
        private string connectionString;

        public StoreDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
            Console.WriteLine("Connection String: " + connectionString);
        }

        public void CreateStoreTable()
        {
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    Console.WriteLine("Database connection opened.");

                    string query = @"
                    CREATE TABLE IF NOT EXISTS Store (
                        ID INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Email TEXT NOT NULL,
                        PhoneNumber TEXT NOT NULL,
                        Description TEXT,
                        Address TEXT,
                        Rating REAL NOT NULL,
                        Reviews TEXT,
                        Complaints TEXT
                    );";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        command.ExecuteNonQuery();
                        Console.WriteLine("Store table created.");
                    }
                    connection.Close();

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while creating the Store table: " + ex.Message);
            }
        }

        public List<StoreDTO> GetAllStores()
        {
            List<StoreDTO> stores = new List<StoreDTO>();
            try
            {
                using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM Store;";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                stores.Add(new StoreDTO
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    Name = reader["Name"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    Description = reader["Description"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    Rating = Convert.ToDouble(reader["Rating"]),
                                    Reviews = reader["Reviews"].ToString(),
                                    Complaints = reader["Complaints"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred while retrieving all stores: " + ex.Message);
            }
            return stores;
        }

        public void AddStore(StoreDTO store)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                INSERT INTO Store (Name, Email, PhoneNumber, Description, Address, Rating, Reviews, Complaints) 
                VALUES (@Name, @Email, @PhoneNumber, @Description, @Address, @Rating, @Reviews, @Complaints);";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", store.Name);
                    command.Parameters.AddWithValue("@Email", store.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", store.PhoneNumber);
                    command.Parameters.AddWithValue("@Description", store.Description);
                    command.Parameters.AddWithValue("@Address", store.Address);
                    command.Parameters.AddWithValue("@Rating", store.Rating);
                    command.Parameters.AddWithValue("@Reviews", string.Join(";", store.Reviews));
                    command.Parameters.AddWithValue("@Complaints", string.Join(";", store.Complaints));
                    command.ExecuteNonQuery();
                    Console.WriteLine("Store added to the table.");
                }
            }
        }

        public void UpdateStore(StoreDTO store)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = @"
                UPDATE Store 
                SET Name = @Name, 
                    Email = @Email, 
                    PhoneNumber = @PhoneNumber, 
                    Description = @Description, 
                    Address = @Address, 
                    Rating = @Rating, 
                    Reviews = @Reviews, 
                    Complaints = @Complaints 
                WHERE ID = @ID;";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", store.Name);
                    command.Parameters.AddWithValue("@Email", store.Email);
                    command.Parameters.AddWithValue("@PhoneNumber", store.PhoneNumber);
                    command.Parameters.AddWithValue("@Description", store.Description);
                    command.Parameters.AddWithValue("@Address", store.Address);
                    command.Parameters.AddWithValue("@Rating", store.Rating);
                    command.Parameters.AddWithValue("@Reviews", string.Join(";", store.Reviews));
                    command.Parameters.AddWithValue("@Complaints", string.Join(";", store.Complaints));
                    command.Parameters.AddWithValue("@ID", store.ID);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Store updated.");
                }
            }
        }

        public void DeleteStore(int storeId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "DELETE FROM Store WHERE ID = @ID;";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", storeId);
                    command.ExecuteNonQuery();
                    Console.WriteLine("Store deleted.");
                }
            }
        }

        public StoreDTO GetStore(int storeId)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM Store WHERE ID = @ID;";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ID", storeId);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine("Store retrieved.");
                            return new StoreDTO
                            {
                                ID = Convert.ToInt32(reader["ID"]),
                                Name = reader["Name"].ToString(),
                                Email = reader["Email"].ToString(),
                                PhoneNumber = reader["PhoneNumber"].ToString(),
                                Description = reader["Description"].ToString(),
                                Address = reader["Address"].ToString(),
                                Rating = Convert.ToDouble(reader["Rating"]),
                                Reviews = reader["Reviews"].ToString(),
                                Complaints = reader["Complaints"].ToString()
                            };
                        }
                    }
                }
            }
            return null;
        }
    }
}
