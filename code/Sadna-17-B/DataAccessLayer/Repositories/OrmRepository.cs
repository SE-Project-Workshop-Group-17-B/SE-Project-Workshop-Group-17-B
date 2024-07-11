using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ServiceStack.OrmLite;
using System.Collections.Generic;
using System.Data;

namespace Sadna_17_B.DataAccessLayer.Repositories
{
    public class OrmRepository<T>
    {
        public void CreateTable()
        {
            using (IDbConnection db = OrmLiteHelper.GetInstance().OpenConnection())
            {
                db.CreateTableIfNotExists<T>();
            }
        }

        public void DropTable()
        {
            using (IDbConnection db = OrmLiteHelper.GetInstance().OpenConnection())
            {
                db.DropTable<T>();
            }
        }

        public void DropAndCreateTable()
        {
            using (IDbConnection db = OrmLiteHelper.GetInstance().OpenConnection())
            {
                db.DropAndCreateTable<T>();
            }
        }

        public void Add(T entry)
        {
            using (IDbConnection db = OrmLiteHelper.GetInstance().OpenConnection())
            {
                db.Insert(entry);
            }
        }

        public List<T> GetAll()
        {
            using (IDbConnection db = OrmLiteHelper.GetInstance().OpenConnection())
            {
                return db.Select<T>();
            }
        }

        public int Delete(T entry)
        {
            using (IDbConnection db = OrmLiteHelper.GetInstance().OpenConnection())
            {
                return db.Delete<T>(entry);
            }
        }

        public int DeleteById(object id)
        {
            using (IDbConnection db = OrmLiteHelper.GetInstance().OpenConnection())
            {
                return db.DeleteById<T>(id);
            }
        }
    }
}