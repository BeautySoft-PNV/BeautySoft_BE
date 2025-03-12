using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.AspNetCore.Mvc;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly IPaymentService _vnPayService;
        private readonly ApplicationDbContext _context;
        public PaymentController(IPaymentService vnPayService, ApplicationDbContext context)
        {
		
            _vnPayService = vnPayService;
            _context = context;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return  Ok(new { paymentUrl = url });
        }
        
        [HttpGet("confirm")]
        public async Task<IActionResult> ConfirmPayment()
        {
            string userId = Request.Query["userId"];
            string amount = Request.Query["vnp_Amount"];
            string bankCode = Request.Query["vnp_BankCode"];
            string bankTranNo = Request.Query["vnp_BankTranNo"];
            string cardType = Request.Query["vnp_CardType"];
            string orderInfo = Request.Query["vnp_OrderInfo"];
            string payDate = Request.Query["vnp_PayDate"];
            string responseCode = Request.Query["vnp_ResponseCode"];
            string transactionNo = Request.Query["vnp_TransactionNo"];
            string transactionStatus = Request.Query["vnp_TransactionStatus"];
            string txnRef = Request.Query["vnp_TxnRef"];
            int typeStorageId = int.Parse(Request.Query["typeStorageId"]);
            var typeStorage = await _context.TypeStorages.FindAsync(typeStorageId);
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(payDate))
            {
                return BadRequest("Lỗi: Dữ liệu không hợp lệ.");
            }

            if (!DateTime.TryParseExact(payDate, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out DateTime dateTimeStart))
            {
                return BadRequest("Lỗi: Định dạng ngày tháng không hợp lệ.");
            }
            DateTime dateTimeEnd = dateTimeStart.AddMonths(1);

            if (responseCode == "00" && transactionStatus == "00")
            {
                var payment = new ManagerStorageModel()
                {
                    UserId = int.Parse(userId),
                    TypeStorageId = typeStorageId,
                    DateTimeStart = dateTimeStart,
                    DateTimeEnd = dateTimeEnd
                };

                _context.ManagerStorages.Add(payment);
                await _context.SaveChangesAsync();
                
                var notification = new NotificationHistoryModel()
                {
                    UserId = int.Parse(userId),
                    NotificationId = 3,
                    Title = "Payment"
                };

                _context.NotificationHistories.Add(notification);
                await _context.SaveChangesAsync();
                bool isSuccess = (responseCode == "00" && transactionStatus == "00");
                ViewBag.IsSuccess = isSuccess;
                ViewBag.TypeStorageName = typeStorage?.Name;
                ViewBag.Amount = amount;
                ViewBag.BankCode = bankCode;
                ViewBag.BankTranNo = bankTranNo;
                ViewBag.CardType = cardType;
                ViewBag.OrderInfo = orderInfo;
                ViewBag.PayDate = payDate;
                ViewBag.TransactionNo = transactionNo;
                ViewBag.TxnRef = txnRef;
                return View("PaymentResult");
            }
            else
            {
                ViewBag.Message = "Thanh toán thất bại. Vui lòng thử lại.";
                ViewBag.IsSuccess = false;
                return View("PaymentResult");
            }
        }
    }
}