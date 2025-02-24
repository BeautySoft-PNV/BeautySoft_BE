using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace BeautySoftBE.Utils
{
    public class VnPayLibrary
    {
        private SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }
        
        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            StringBuilder data = new StringBuilder();
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (data.Length > 0)
                {
                    data.Append("&");
                }
                data.Append(HttpUtility.UrlEncode(kv.Key) + "=" + HttpUtility.UrlEncode(kv.Value));
            }

            string queryString = data.ToString();
            string signData = queryString + "&vnp_SecureHash=" + ComputeHmacSHA512(vnp_HashSecret, queryString);
            return baseUrl + "?" + signData;
        }
        
        public bool ValidateSignature(IQueryCollection queryData, string vnp_HashSecret)
        {
            var secureHash = queryData["vnp_SecureHash"];
            var checkData = queryData
                .Where(x => x.Key != "vnp_SecureHash")
                .OrderBy(x => x.Key)
                .Select(x => $"{x.Key}={x.Value}")
                .Aggregate((a, b) => $"{a}&{b}");

            var computedHash = ComputeHmacSHA512(vnp_HashSecret, checkData);
            return string.Equals(secureHash, computedHash, StringComparison.InvariantCultureIgnoreCase);
        }
        
        private string ComputeHmacSHA512(string key, string data)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(key)))
            {
                byte[] hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }

    public class VnPayCompare : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            return string.CompareOrdinal(x, y);
        }
    }
}
