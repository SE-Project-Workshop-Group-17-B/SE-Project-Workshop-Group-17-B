using Sadna_17_B.Data;
using Sadna_17_B.ServiceLayer.Services;

public class ServiceFactory
{
    public IUserService UserService { get; private set; }
    public IStoreService StoreService { get; private set; }
    private readonly UnitOfWork _unitOfWork;

    public ServiceFactory()
    {
        _unitOfWork = new UnitOfWork();
        BuildInstances();
        InitializeDatabase();
    }

    private void BuildInstances()
    {
        UserService = new UserService(_unitOfWork);
        StoreService = new StoreService(_unitOfWork);
    }

    private void InitializeDatabase()
    {
        using (var context = new TradingSystemContext())
        {
            if (!context.Users.Any())
            {
                GenerateData();
                _unitOfWork.SaveChanges();
            }
        }
    }

    private void GenerateData()
    {
        // Your existing GenerateData method, but using _unitOfWork instead of direct service calls
    }
}