using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Services;

public class ManagerStorageService : IManagerStorageService
{
    private readonly ApplicationDbContext _context;
    
    public ManagerStorageService(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<PaymentModel>> GetAllPaymentsAsync()
    {
        return await _context.Payments
            .Include(p => p.TypeStorage)
            .Include(p => p.User)
            .Select(p => new PaymentModel
            {
                Id = p.Id,
                TypeStorageId = p.TypeStorageId,
                UserId = p.UserId,
                DateTimeStart = p.DateTimeStart,
                DateTimeEnd = p.DateTimeEnd,
                TypeStorage = p.TypeStorage != null ? new TypeStorageModel
                {
                    Id = p.TypeStorage.Id,
                    Name = p.TypeStorage.Name,
                    Price = p.TypeStorage.Price,
                    Description = p.TypeStorage.Description
                } : null, 
                User = p.User != null ? new UserModel
                {
                    Id = p.User.Id,
                    Name = p.User.Name,
                    Email = p.User.Email,
                } : null 
            })
            .ToListAsync();
    }
}