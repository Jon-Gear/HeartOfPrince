using System;
using System.Reflection;

namespace HeartOfPrince.Domain
{

    public static class GameStateDebugConversion
    {
        public static CharacterID ToCharacterId(string raw)
        {
            return ConvertFromString<CharacterID>(raw);
        }

        public static TopicName ToTopicName(string raw)
        {
            return ConvertFromString<TopicName>(raw);
        }

        private static T ConvertFromString<T>(string raw)
        {
            var targetType = typeof(T);

            if (targetType == typeof(string))
                return (T)(object)raw;

            if (targetType.IsEnum)
                return (T)Enum.Parse(targetType, raw, ignoreCase: true);

            var constructor = targetType.GetConstructor(new[] { typeof(string) });
            if (constructor != null)
                return (T)constructor.Invoke(new object[] { raw });

            var fromStringMethod = targetType.GetMethod(
                "FromString",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string) },
                null);

            if (fromStringMethod != null)
                return (T)fromStringMethod.Invoke(null, new object[] { raw });

            var fromMethod = targetType.GetMethod(
                "From",
                BindingFlags.Public | BindingFlags.Static,
                null,
                new[] { typeof(string) },
                null);

            if (fromMethod != null)
                return (T)fromMethod.Invoke(null, new object[] { raw });

            throw new InvalidOperationException(
                $"Cannot convert string '{raw}' to {targetType.Name}. " +
                $"Make {targetType.Name} an enum, add a constructor that takes string, " +
                $"or add a public static FromString(string) method.");
        }
    }
}
