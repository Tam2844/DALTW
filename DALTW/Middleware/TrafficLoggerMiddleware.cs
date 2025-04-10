using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using DALTW.Repositories;

public class TrafficLoggerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IServiceScopeFactory _scopeFactory; // Thay vì inject ITrafficLogRepository
    private readonly ILogger<TrafficLoggerMiddleware> _logger;

    public TrafficLoggerMiddleware(RequestDelegate next, IServiceScopeFactory scopeFactory, ILogger<TrafficLoggerMiddleware> logger)
    {
        _next = next;
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        // Bỏ qua các file tĩnh
        if (context.Request.Path.StartsWithSegments("/css") ||
            context.Request.Path.StartsWithSegments("/js") ||
            context.Request.Path.StartsWithSegments("/images") ||
            context.Request.Path.StartsWithSegments("/favicon.ico"))
        {
            await _next(context);
            return;
        }

        // Kiểm tra Session
        if (!context.Session.Keys.Contains("HasLoggedTraffic"))
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var trafficLogRepository = scope.ServiceProvider.GetRequiredService<ITrafficLogRepository>();
                string ipAddress = context.Connection.RemoteIpAddress?.ToString();
                string url = context.Request.Path;

                await trafficLogRepository.LogVisitAsync(ipAddress, url);

                // Gắn cờ session đã log
                context.Session.SetString("HasLoggedTraffic", "true");
            }
        }

        await _next(context);
    }

}
