using BeautySoftBE.Models;

namespace BeautySoftBE.Services;
public interface IManagerStorageService
{
    Task<List<PaymentModel>> GetAllPaymentsAsync();
}