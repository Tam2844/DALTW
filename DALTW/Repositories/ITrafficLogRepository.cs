using System.Collections.Generic;
using System.Threading.Tasks;
using DALTW.Models;

namespace DALTW.Repositories
{
    public interface ITrafficLogRepository
    {
        Task LogVisitAsync(string ipAddress, string url);
        Task<int> GetTotalVisitsAsync();
    }
}
