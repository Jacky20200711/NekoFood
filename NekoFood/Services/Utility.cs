using System.Security.Cryptography;
using System.Text;

namespace NekoFood.Services
{
    public static class Utility
    {
        public static string GetLoginName(HttpContext httpContext)
        {
            return httpContext.Session.GetString("loginName") ?? "";
        }

        public static string GetEncryptPassword(string password)
        {
            using var md5 = MD5.Create();
            var hashResult = md5.ComputeHash(Encoding.ASCII.GetBytes(password));
            return BitConverter.ToString(hashResult).Replace("-", "");
        }

        /// <returns>return integer if parse success else 1</returns>
        public static int ConvertStrToInt(string content)
        {
            if(int.TryParse(content, out int myInt))
            {
                return myInt;
            }
            return 1;
        }
    }
}
