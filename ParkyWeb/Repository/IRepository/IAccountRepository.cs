using ParkyWeb.Models;
using System.Threading.Tasks;

namespace ParkyWeb.Repository.IRepository
{
    public interface IAccountRepository : IRepository<User>
    {
        Task<User> LoginAsync(string url,User obj);
        Task<bool> RegisterAsync(string url,User obj);
    }
}
