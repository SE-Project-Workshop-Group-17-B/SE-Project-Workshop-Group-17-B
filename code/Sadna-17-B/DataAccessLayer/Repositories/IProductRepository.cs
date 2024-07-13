using Sadna_17_B.DomainLayer.StoreDom;
using System.Collections.Generic;

namespace Sadna_17_B.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        //IEnumerable<Product> GetProductsByStore(int storeId);
    }
}