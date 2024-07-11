using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.DomainLayer.StoreDom;
using System.Collections.Generic;
using System.Linq;

namespace Sadna_17_B.Repositories.Implementations
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        public IEnumerable<Product> GetProductsByStore(int storeId)
        {
            return Context.Products.Where(p => p.store_ID == storeId).ToList();
        }
    }
}