using Dhaba_Delicious.Serializables;
using System.Threading.Tasks;

namespace Dhaba_Delicious.Interfaces
{
    public interface IResponseService
    {
        Task<T> GetResponseAsync<T>(string token, string url,string sessionId) where T : class;

        Task<T> GetResponseWithBody<T>(string userQuery, string token, string url,string sessionId) where T : class;
    }
}
