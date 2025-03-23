using BeautySoftBE.Data;
using BeautySoftBE.Library;
using BeautySoftBE.Models;
using BeautySoftBE.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BeautySoftBE.Services;

public class PaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ApplicationDbContext _context;
    public PaymentService( IConfiguration configuration, ApplicationDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public string CreatePaymentUrl(PaymentInformationModel model, HttpContext context)
    {
        var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]);
        var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
        var tick = DateTime.Now.Ticks.ToString();
        var pay = new VnPayLibrary();
        var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];

        pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]);
        pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]);
        pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]);
        pay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString());
        pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
        pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]);
        pay.AddRequestData("vnp_IpAddr", pay.GetIpAddress(context));
        pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]);
        pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription} {model.Amount}");
        pay.AddRequestData("vnp_OrderType", model.OrderType);
        pay.AddRequestData("vnp_ReturnUrl", model.ReturnUrl);
        pay.AddRequestData("vnp_TxnRef", tick);

        var paymentUrl =
            pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"], _configuration["Vnpay:HashSecret"]);

        return paymentUrl;
    }
    public PaymentResponseModel PaymentExecute(IQueryCollection collections)
    {
        var hashSecret = _configuration["Vnpay:HashSecret"];
        if (string.IsNullOrEmpty(hashSecret))
        {
            throw new Exception("HashSecret is missing from configuration.");
        }

        var pay = new VnPayLibrary();
        var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]);
        if (response == null)
        {
            throw new Exception("GetFullResponseData returned null.");
        }
        return response;
    }
    
}
