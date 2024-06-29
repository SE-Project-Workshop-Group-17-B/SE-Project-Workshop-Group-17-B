using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sadna_17_B.DataAccess.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        IEnumerable<Product> GetByStore(int storeId);
        IEnumerable<Product> GetByCategory(string category);
    }
}
