using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Web;

namespace Sadna_17_B.DataAccessLayer.store
{
    public class PurchasePolicyDAO
    {
        private readonly string connectionString;

        public PurchasePolicyDAO()
        {
            connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
            Console.WriteLine("connection string for PurchasePolicy is: " + connectionString);

        }

        public void CreatePurchasePolicyTable()
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = @"CREATE TABLE IF NOT EXISTS PurchasePolicy (
                        storeID INT NOT NULL,
                        purchaseID INT NOT NULL,
                        PRIMARY KEY (storeID, purchaseID),
                        FOREIGN KEY (storeID) REFERENCES Store(ID),
                        FOREIGN KEY (purchaseID) REFERENCES Purchase(Id)
                    )";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public void AddPurchasePolicy(PurchasePolicyDTO purchasePolicy)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "INSERT INTO PurchasePolicy (storeID, purchaseID) VALUES (@StoreID, @PurchaseID)";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StoreID", purchasePolicy.StoreID);
                    command.Parameters.AddWithValue("@PurchaseID", purchasePolicy.PurchaseID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                connection.Close();

            }
        }

        public PurchasePolicyDTO GetPurchasePolicy(int storeID, int purchaseID)
        {
            PurchasePolicyDTO purchasePolicy = null;

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {

                string query = "SELECT * FROM PurchasePolicy WHERE storeID = @StoreID AND purchaseID = @PurchaseID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StoreID", storeID);
                    command.Parameters.AddWithValue("@PurchaseID", purchaseID);

                    connection.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            purchasePolicy = new PurchasePolicyDTO
                            {
                                StoreID = (int)reader["storeID"],
                                PurchaseID = (int)reader["purchaseID"]
                            };
                        }
                    }
                }
                connection.Close();

            }

            return purchasePolicy;
        }

        public List<PurchasePolicyDTO> GetAllPurchasePolicies()
        {
            List<PurchasePolicyDTO> purchasePolicies = new List<PurchasePolicyDTO>();

            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "SELECT * FROM PurchasePolicy";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    connection.Open();
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            PurchasePolicyDTO purchasePolicy = new PurchasePolicyDTO
                            {
                                StoreID = (int)reader["storeID"],
                                PurchaseID = (int)reader["purchaseID"]
                            };

                            purchasePolicies.Add(purchasePolicy);
                        }
                    }
                }
                connection.Close();

            }

            return purchasePolicies;
        }

        public void UpdatePurchasePolicy(PurchasePolicyDTO purchasePolicy)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                string query = "UPDATE PurchasePolicy SET storeID = @StoreID, purchaseID = @PurchaseID " +
                               "WHERE storeID = @StoreID AND purchaseID = @PurchaseID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StoreID", purchasePolicy.StoreID);
                    command.Parameters.AddWithValue("@PurchaseID", purchasePolicy.PurchaseID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                connection.Close();

            }
        }

        public void DeletePurchasePolicy(int storeID, int purchaseID)
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {

                string query = "DELETE FROM PurchasePolicy WHERE storeID = @StoreID AND purchaseID = @PurchaseID";

                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StoreID", storeID);
                    command.Parameters.AddWithValue("@PurchaseID", purchaseID);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
                connection.Close();

            }
        }
    }
}