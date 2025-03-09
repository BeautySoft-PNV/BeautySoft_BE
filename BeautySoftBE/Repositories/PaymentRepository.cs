using BeautySoftBE.Data;
using BeautySoftBE.Models;

namespace BeautySoftBE.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly ApplicationDbContext _context;
    public PaymentRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void SavePayment(PaymentModel payment)
    {
        _context.Payments.Add(payment);
        _context.SaveChanges();
    }
}