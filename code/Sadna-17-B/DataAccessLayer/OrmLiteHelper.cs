using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.OrmLite;
using ServiceStack.OrmLite.Sqlite;
using System.Configuration;
using System.Data;


namespace Sadna_17_B.DataAccessLayer
{
    public class OrmLiteHelper
    {
        public static bool memoryDB = false;
        private static OrmLiteConnectionFactory _dbFactory;
        private static OrmLiteHelper instance;

        private OrmLiteHelper()
        {
            string connectionName = memoryDB ? "SQLiteDB-Memory" : "SQLiteDB"; // Can be used to swap the database to an in-memory one
            // Ensure the provider is only registered once
            var connectionString = ConfigurationManager.ConnectionStrings[connectionName]?.ConnectionString;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(connectionName + " connection string is missing in the Web.config file.");
            }

            _dbFactory = new OrmLiteConnectionFactory(connectionString, SqliteDialect.Provider);
        }

        public static OrmLiteHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new OrmLiteHelper();
            }
            return instance;
        }

        public IDbConnection OpenConnection()
        {
            return _dbFactory.OpenDbConnection();
        }
    }
}