using Sadna_17_B.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.DataAccess.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(TradingSystemContext context) : base(context) { }

        public IEnumerable<Product> GetByStore(int storeId)
        {
            return _dbSet.Where(p => p.StoreID == storeId).ToList();
        }

        public IEnumerable<Product> GetByCategory(string category)
        {
            return _dbSet.Where(p => p.Category == category).ToList();
        }
    }
}
