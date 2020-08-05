using System;
using System.Text;

namespace DocuStore.Shared
{
    public static class TypeHelper
    {
        public static T ConvertTo<T>(this object value)
        {
            return ConvertTo(value, default(T));
        }

        public static T ConvertTo<T>(this object value, T defaultValue)
        {
            if (value == null || value == DBNull.Value)
                return defaultValue;

            return (T) Convert.ChangeType(value, typeof(T));
        }

        public static string GetDescription<T>()
        {
            var type = typeof(T);

            var name = type.Name;

            var value = string.Empty;

            var previousWasUpper = true;

            foreach (var character in name.ToCharArray())
            {
                if (char.IsUpper(character))
                {
                    if (!previousWasUpper)
                    {
                        value += " ";
                    }

                    previousWasUpper = true;
                }

                value += character;
            }

            return value;
        }

        public static string GetFriendlyName(this Type type)
        {
            if (!type.IsGenericType)
                return type.Name;

            var friendlyName = new StringBuilder();
            friendlyName.Append(type.Name.Replace("`", ""));
            friendlyName.Append("<");

            var typeParameters = type.GetGenericArguments();

            for (var i = 0; i < typeParameters.Length; ++i)
            {
                var typeParamName = GetFriendlyName(typeParameters[i]);
                friendlyName.Append(i == 0 ? typeParamName : "," + typeParamName);
            }

            friendlyName.Append(">");

            return friendlyName.ToString();
        }
    }
}