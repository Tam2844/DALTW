using System.Threading.Tasks;

using DALTW.Models;
using Microsoft.EntityFrameworkCore;

namespace DALTW.Repositories
{
    public class TrafficLogRepository : ITrafficLogRepository
    {
        private readonly ApplicationDbContext _context;

        public TrafficLogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task LogVisitAsync(string ipAddress, string url)
        {
            var log = new TrafficLog { IpAddress = ipAddress, Url = url };
            _context.TrafficLogs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task<int> GetTotalVisitsAsync()
        {
            return await _context.TrafficLogs.CountAsync();
        }
    }
}
