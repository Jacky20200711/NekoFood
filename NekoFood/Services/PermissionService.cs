namespace NekoFood.Services
{
    public static class PermissionService
    {
        public static bool HasPermissionToModifyBentoGroup(HttpContext httpContext, string creator)
        {
            if (httpContext.Session.GetString("admin") != null)
            {
                return true;
            }

            return Utility.GetLoginName(httpContext) == creator;
        }
    }
}
