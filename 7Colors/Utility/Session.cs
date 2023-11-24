using Newtonsoft.Json;

namespace _7Colors.Utility
{
    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string Key, T value)
        {
            session.SetString(Key, JsonConvert.SerializeObject(value));
        }

        public static T Get<T>(this ISession session, string key)
        {
            var value = session.GetString(key);

            return value == null ? default(T)! :
                JsonConvert.DeserializeObject<T>(value)!;

        }

    }
}
