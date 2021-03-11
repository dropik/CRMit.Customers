using System.Threading.Tasks;

namespace CRMit.Customers.Services
{
    public interface IStartupTask
    {
        Task ExecuteAsync();
    }
}
