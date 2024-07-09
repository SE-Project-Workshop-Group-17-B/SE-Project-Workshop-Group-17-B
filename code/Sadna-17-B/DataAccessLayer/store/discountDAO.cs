using Sadna_17_B.DataAccessLayer.store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Data.SQLite;

public class DiscountDAO
{
    private readonly string connectionString;

    public DiscountDAO()
    {
        connectionString = ConfigurationManager.ConnectionStrings["SQLiteDB"].ConnectionString;
        Console.WriteLine("connection string for discount is: " + connectionString);
    }

    public void CreateDiscountTable()
    {
        try
        {
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();
                Console.WriteLine("Database connection opened.");

                string query = @"
                CREATE TABLE IF NOT EXISTS Discount (
                    DiscountID INTEGER PRIMARY KEY AUTOINCREMENT,
                    StartDate TEXT NOT NULL,
                    EndDate TEXT NOT NULL,
                    Strategy TEXT NOT NULL,
                    DiscountType TEXT NOT NULL,
                    Relevant TEXT NOT NULL,
                    Conditions TEXT NOT NULL
                );";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.ExecuteNonQuery();
                    Console.WriteLine("Discount table created.");
                }
                connection.Close();

            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred while creating the Discount table: " + ex.Message);
        }
    }

    public void AddDiscount(DiscountDTO discount)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO Discount (DiscountID,StartDate,EndDate,Strategy,DiscountType,Relevant,Conditions) " +
                           "VALUES (@DiscountID, @StartDate, @EndDate, @Strategy, @DiscountType, @Relevant, @Conditions)";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DiscountID", discount.DiscountID);
                command.Parameters.AddWithValue("@StartDate", discount.StartDate);
                command.Parameters.AddWithValue("@EndDate", discount.EndDate);
                command.Parameters.AddWithValue("@Strategy", discount.Strategy);
                command.Parameters.AddWithValue("@DiscountType", discount.DiscountType);
                command.Parameters.AddWithValue("@Relevant", discount.Relevant);
                command.Parameters.AddWithValue("@Conditions", discount.Conditions);

                command.ExecuteNonQuery();
            }
            connection.Close();
        }
    }

    public DiscountDTO GetDiscount(int discountID)
    {
        DiscountDTO discount = null;

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {

            string query = "SELECT * FROM Discount WHERE DiscountID = @DiscountID";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                connection.Open();

                command.Parameters.AddWithValue("@DiscountID", discountID);
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        Console.WriteLine("reader can read");
                        discount = new DiscountDTO
                        {
                            DiscountID = reader["DiscountID"] != DBNull.Value ? Convert.ToInt32(reader["DiscountID"]) : discountID,
                            StartDate = reader["StartDate"] != DBNull.Value ? Convert.ToString(reader["StartDate"]) : "string.Empty",
                            EndDate = reader["EndDate"] != DBNull.Value ? Convert.ToString(reader["EndDate"]) : "string.Empty",
                            Strategy = reader["Strategy"] != DBNull.Value ? Convert.ToString(reader["Strategy"]) : "string.Empty",
                            DiscountType = reader["DiscountType"] != DBNull.Value ? Convert.ToString(reader["DiscountType"]) : "string.Empty",
                            Relevant = reader["Relevant"] != DBNull.Value ? Convert.ToString(reader["Relevant"]) : " string.Empty",
                            Conditions = reader["Conditions"] != DBNull.Value ? Convert.ToString(reader["Conditions"]) : "string.Empty"
                        };
                    }
                    else
                    {
                        Console.WriteLine("No rows found.");
                    }
                }
                connection.Close();

            }
        }
        return discount;
    }


    public void UpdateDiscount(DiscountDTO discount)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {
            string query = "UPDATE Discount SET startDate = @StartDate, endDate = @EndDate, Strategy = @Strategy, " +
                           "discountType = @DiscountType, relevant = @Relevant, conditions = @Conditions " +
                           "WHERE DiscountID = @DiscountID";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DiscountID", discount.DiscountID);
                command.Parameters.AddWithValue("@StartDate", discount.StartDate);
                command.Parameters.AddWithValue("@EndDate", discount.EndDate);
                command.Parameters.AddWithValue("@Strategy", discount.Strategy);
                command.Parameters.AddWithValue("@DiscountType", discount.DiscountType);
                command.Parameters.AddWithValue("@Relevant", discount.Relevant);
                command.Parameters.AddWithValue("@Conditions", discount.Conditions);

                connection.Open();
                command.ExecuteNonQuery();
            }
            connection.Close();

        }
    }

    public void DeleteDiscount(int discountID)
    {
        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {

            string query = "DELETE FROM Discount WHERE DiscountID = @DiscountID";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@DiscountID", discountID);

                connection.Open();
                command.ExecuteNonQuery();
            }
            connection.Close();

        }
    }

    public List<DiscountDTO> GetAllDiscounts()
    {
        List<DiscountDTO> discounts = new List<DiscountDTO>();

        using (SQLiteConnection connection = new SQLiteConnection(connectionString))
        {

            string query = "SELECT * FROM Discount";

            using (SQLiteCommand command = new SQLiteCommand(query, connection))
            {
                connection.Open();
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DiscountDTO discount = new DiscountDTO
                        {
                            DiscountID = (int)reader["DiscountID"],
                            StartDate = reader["startDate"].ToString(),
                            EndDate = reader["endDate"].ToString(),
                            Strategy = reader["Strategy"].ToString(),
                            DiscountType = reader["discountType"].ToString(),
                            Relevant = reader["relevant"].ToString(),
                            Conditions = reader["conditions"].ToString()
                        };

                        discounts.Add(discount);
                    }
                }
            }
            connection.Close();

        }

        return discounts;
    }
}
