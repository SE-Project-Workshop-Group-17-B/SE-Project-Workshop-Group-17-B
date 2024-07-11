using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.DataAccessLayer.store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

public class DiscountPolicyDAO
{
    private readonly string connectionString;

    public DiscountPolicyDAO()
    {
        string connectionName = OrmLiteHelper.memoryDB ? "SQLiteDB-Memory" : "SQLiteDB";
        connectionString = ConfigurationManager.ConnectionStrings[connectionName].ConnectionString;
    }

    public void CreateDiscountPolicyTable()
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            string query = @"CREATE TABLE IF NOT EXISTS DiscountPolicy (
                        storeID INT NOT NULL,
                        discountID INT NOT NULL,
                        PRIMARY KEY (storeID, discountID),
                        FOREIGN KEY (storeID) REFERENCES Store(ID),
                        FOREIGN KEY (discountID) REFERENCES Discount(DiscountID)
                    )";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                connection.Open();
                command.ExecuteNonQuery();
            }
        }

    }

    public void AddDiscountPolicy(DiscountPolicyDTO discountPolicy)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            string query = "INSERT INTO DiscountPolicy (storeID, discountID) VALUES (@StoreID, @DiscountID)";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StoreID", discountPolicy.StoreID);
                command.Parameters.AddWithValue("@DiscountID", discountPolicy.DiscountID);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    public DiscountPolicyDTO GetDiscountPolicy(int storeID, int discountID)
    {
        DiscountPolicyDTO discountPolicy = null;

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            string query = "SELECT * FROM DiscountPolicy WHERE storeID = @StoreID AND discountID = @DiscountID";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StoreID", storeID);
                command.Parameters.AddWithValue("@DiscountID", discountID);

                connection.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        discountPolicy = new DiscountPolicyDTO
                        {
                            StoreID = (int)reader["storeID"],
                            DiscountID = (int)reader["discountID"]
                        };
                    }
                }
            }
        }

        return discountPolicy;
    }

    public void UpdateDiscountPolicy(DiscountPolicyDTO discountPolicy)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            string query = "UPDATE DiscountPolicy SET storeID = @StoreID, discountID = @DiscountID " +
                           "WHERE storeID = @StoreID AND discountID = @DiscountID";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StoreID", discountPolicy.StoreID);
                command.Parameters.AddWithValue("@DiscountID", discountPolicy.DiscountID);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }

    public void DeleteDiscountPolicy(int storeID, int discountID)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            string query = "DELETE FROM DiscountPolicy WHERE storeID = @StoreID AND discountID = @DiscountID";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@StoreID", storeID);
                command.Parameters.AddWithValue("@DiscountID", discountID);

                connection.Open();
                command.ExecuteNonQuery();
            }
        }
    }
    

    public List<DiscountPolicyDTO> GetAllDiscountPolicies()
    {
        List<DiscountPolicyDTO> discountPolicies = new List<DiscountPolicyDTO>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            string query = "SELECT * FROM DiscountPolicy";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                connection.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DiscountPolicyDTO discountPolicy = new DiscountPolicyDTO
                        {
                            StoreID = (int)reader["storeID"],
                            DiscountID = (int)reader["discountID"]
                        };

                        discountPolicies.Add(discountPolicy);
                    }
                }
            }
        }

        return discountPolicies;
    }
}
