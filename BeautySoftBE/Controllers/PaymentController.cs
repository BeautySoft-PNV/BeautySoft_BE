using System.IdentityModel.Tokens.Jwt;
using BeautySoftBE.Data;
using BeautySoftBE.Middleware;
using BeautySoftBE.Models;
using BeautySoftBE.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _vnPayService;
        private readonly ApplicationDbContext _context;
        private readonly IManagerStorageService _storageService;
        public PaymentController(IPaymentService vnPayService, ApplicationDbContext context, IManagerStorageService storageService)
        {
            _vnPayService = vnPayService;
            _context = context;
            _storageService = storageService;
        }

        public IActionResult CreatePaymentUrlVnpay(PaymentInformationModel model)
        {
            var url = _vnPayService.CreatePaymentUrl(model, HttpContext);

            return  Ok(new { paymentUrl = url });
        }
        
        [HttpGet("all")]
        public async Task<IActionResult> GetPaymentAll()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString();
            
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7).Trim();
            }
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "role")?.Value;

            if (role != "ADMIN")
            {
                return BadRequest("No right.");
            }
            var payments = await _storageService.GetAllPaymentsAsync();

            if (payments == null || payments.Count == 0)
            {
                return Ok(new List<object>()); 
            }

            return Ok(payments);
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
                return BadRequest("Error: Invalid date format.");
            }
            DateTime dateTimeEnd = dateTimeStart.AddMonths(1);

            if (responseCode == "00" && transactionStatus == "00")
            {
                var payment = new PaymentModel()
                {
                    UserId = int.Parse(userId),
                    TypeStorageId = typeStorageId,
                    DateTimeStart = dateTimeStart,
                    DateTimeEnd = dateTimeEnd
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();
                
                var notification = new NotificationHistoryModel()
                {
                    UserId = int.Parse(userId),
                    NotificationId = 1,
                    Title = "Payment"
                };
                var isExist = await _context.NotificationHistories
                    .AnyAsync(n => n.UserId == notification.UserId && n.Title == notification.Title);
                if (!isExist)
                {
                    _context.NotificationHistories.Add(notification);
                    await _context.SaveChangesAsync();
                }
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
                ViewBag.Message = "Payment failed. Please try again.";
                ViewBag.IsSuccess = false;
                return View("PaymentResult");
            }
        }
    }
}