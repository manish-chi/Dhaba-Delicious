using System.Threading.Tasks;

namespace Dhaba_Delicious.Interfaces
{
    public interface IAuthService
    {
        Task<string> GetAdminToken();
    }
}
