using System.Security.Cryptography;
using System.Text;

namespace NekoFood.Services
{
    public static class Utility
    {
        public static string GetLoginName(HttpContext httpContext)
        {
            return httpContext.Session.GetString("loginName") ?? "unknown";
        }

        public static string GetEncryptPassword(string password)
        {
            using var md5 = MD5.Create();
            var hashResult = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
            return BitConverter.ToString(hashResult).Replace("-", "");
        }
    }
}
