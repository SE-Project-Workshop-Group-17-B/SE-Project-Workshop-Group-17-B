using Sadna_17_B.DataAccessLayer;
using Sadna_17_B.DomainLayer.User;
using System.Linq;

namespace Sadna_17_B.Repositories.Implementations
{
    public class UserRepository : Repository<Subscriber>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public Subscriber GetByUsername(string username)
        {
            return Context.Subscribers.FirstOrDefault(u => u.Username == username);
        }

        public Guest GetGuestById(int guestId)
        {
            return Context.Guests.Find(guestId);
        }

        public void AddGuest(Guest guest)
        {
            Context.Guests.Add(guest);
        }

        public void RemoveGuest(int guestId)
        {
            var guest = Context.Guests.Find(guestId);
            if (guest != null)
            {
                Context.Guests.Remove(guest);
            }
        }
    }
}