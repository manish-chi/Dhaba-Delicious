using Dhaba_Delicious.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Models
{
    public class AuthenticationManager
    {
        private IAuthService _authService;
        public AuthenticationManager(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task<string> AuthenticateAdmin()
        {
            return await _authService.GetAdminToken();
        }
    }
}
