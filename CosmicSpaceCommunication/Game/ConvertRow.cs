using System;

namespace CosmicSpaceCommunication.Game
{
    public class ConvertRow
    {
        /// <summary>
        /// Konwersacja obiektu z bazy na szczegolny typ
        /// </summary>
        public static T Row<T>(object obj)
        {
            var t = typeof(T);

            if (t.IsGenericType && t.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (t == null)
                    return default;
                t = Nullable.GetUnderlyingType(t);
            }

            if (Convert.IsDBNull(obj))
                return default;
            return (T)Convert.ChangeType(obj, t);
        }
    }
}