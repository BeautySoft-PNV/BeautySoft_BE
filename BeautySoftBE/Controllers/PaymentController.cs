using BeautySoftBE.Data;
using BeautySoftBE.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BeautySoftBE.Utils;

namespace BeautySoftBE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        
        [Authorize]
        [HttpPost("create")]
        public IActionResult CreatePayment([FromBody] PaymentRequest request)
        {
            try
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Không thể xác định UserId từ token" });
                }

                var vnp_TmnCode = _configuration["VnPay:TmnCode"];
                var vnp_HashSecret = _configuration["VnPay:HashSecret"];
                var vnp_Url = _configuration["VnPay:Url"];
                var vnp_ReturnUrl = _configuration["VnPay:ReturnUrl"];

                var orderId = DateTime.UtcNow.Ticks.ToString(); 
                var vnpay = new VnPayLibrary();

                vnpay.AddRequestData("vnp_Version", "2.1.0");
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", (request.Amount * 100).ToString()); 
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_TxnRef", orderId);
                vnpay.AddRequestData("vnp_OrderInfo", "Thanh toán đơn hàng " + orderId);
                vnpay.AddRequestData("vnp_OrderType", "billpayment");
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
                vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString());

                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
               
                return Ok(new
                {
                    paymentUrl,
                    orderId,
                    typeStorageId = request.TypeStorageId 
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Lỗi tạo thanh toán", error = ex.Message });
            }
        }

        [HttpGet("callback")]
        public IActionResult PaymentCallback()
        {
            var vnp_HashSecret = _configuration["VnPay:HashSecret"];
            var vnpay = new VnPayLibrary();

            var vnp_ResponseCode = Request.Query["vnp_ResponseCode"];
            var vnp_TxnRef = Request.Query["vnp_TxnRef"];
            var vnp_Amount = Request.Query["vnp_Amount"];
            var vnp_SecureHash = Request.Query["vnp_SecureHash"];

            if (!vnpay.ValidateSignature(Request.Query, vnp_HashSecret))
            {
                return BadRequest(new { message = "Invalid signature" });
            }

            if (vnp_ResponseCode == "00") 
            {
                var userId = GetUserIdFromToken();
                if (userId == null)
                {
                    return Unauthorized(new { message = "Không thể xác định UserId từ token" });
                }

                var typeStorageId = int.TryParse(Request.Query["TypeStorageId"], out int tsId) ? tsId : 1;

                var payment = new PaymentModel
                {
                    TypeStorageId = typeStorageId,
                    UserId = (int)userId,
                    Price = decimal.Parse(vnp_Amount) / 100,
                    PaymentDate = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                _context.SaveChanges();

                return Ok(new { message = "Thanh toán thành công!", orderId = vnp_TxnRef });
            }

            return BadRequest(new { message = "Thanh toán thất bại!", responseCode = vnp_ResponseCode });
        }
    }
}
