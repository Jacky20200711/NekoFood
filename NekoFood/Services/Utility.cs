namespace NekoFood.Services
{
    public static class Utility
    {
        public static string? GetLoginName(HttpContext httpContext)
        {
            return httpContext.Session.GetString("loginName");
        }
    }
}
