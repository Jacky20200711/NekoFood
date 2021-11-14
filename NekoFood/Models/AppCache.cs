namespace NekoFood.Models
{
    public static class AppCache
    {
        private static readonly Dictionary<string, string> cache = new();

        public static void Set(string? key, string value)
        {
            if (key != null)
            {
                cache[key] = value;
            }
        }

        public static string? Get(string? key)
        {
            if (key == null)
            {
                return null;
            }

            return cache.ContainsKey(key) ? cache[key] : null;
        }

        public static void Remove(string? key)
        {
            if (key != null)
            {
                cache.Remove(key);
            }
        }
    }
}
