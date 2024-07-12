using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.DomainLayer.StoreDom;
using System.Linq;

namespace Sadna_17_B.Repositories.Implementations
{
    public class StoreRepository : Repository<Store>, IStoreRepository
    {
        public StoreRepository(ApplicationDbContext context) : base(context)
        {
        }

        //public Store GetStoreWithProducts(int id)
        //{
        //    return Context.Stores
        //        .Include("Products")
        //        .SingleOrDefault(s => s.ID == id);
        //}
    }
}