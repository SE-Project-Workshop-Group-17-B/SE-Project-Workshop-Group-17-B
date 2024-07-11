using Sadna_17_B.DomainLayer.User;

namespace Sadna_17_B.Repositories
{
    public interface IUserRepository : IRepository<Subscriber>
    {
        Subscriber GetByUsername(string username);
        Guest GetGuestById(int guestId);
        void AddGuest(Guest guest);
        void RemoveGuest(int guestId);
    }
}