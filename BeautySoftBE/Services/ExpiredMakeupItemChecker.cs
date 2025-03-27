using BeautySoftBE.Data;
using BeautySoftBE.Models;

namespace BeautySoftBE.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ExpiredMakeupItemChecker : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public ExpiredMakeupItemChecker(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckExpiredMakeupItems();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] {ex.Message}");
            }
            
            await Task.Delay(TimeSpan.FromSeconds(60), stoppingToken);
        }
    }

    private async Task CheckExpiredMakeupItems()
    {
        using (var scope = _scopeFactory.CreateScope())
        {
            var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var expiredItems = await _context.MakeupItems
                .Where(item => item.ExpirationDate <= DateTime.UtcNow)
                .ToListAsync();

            foreach (var item in expiredItems)
            {
                var exists = await _context.NotificationHistories
                    .AnyAsync(n => n.UserId == item.UserId && n.Title == $"Makeup item '{item.Name}' has expired.");

                if (!exists)
                {
                    var notification = new NotificationHistoryModel
                    {
                        UserId = item.UserId,
                        NotificationId = 2,
                        Title = $"Makeup item '{item.Name}' has expired."
                    };

                    _context.NotificationHistories.Add(notification);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
