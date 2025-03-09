using BeautySoftBE.Models;

namespace BeautySoftBE.Repositories;

public interface IPaymentRepository
{
    void SavePayment(PaymentModel payment);
}