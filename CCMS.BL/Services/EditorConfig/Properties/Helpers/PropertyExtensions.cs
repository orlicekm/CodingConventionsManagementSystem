using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CCMS.BL.Services.EditorConfig.Properties.Base;

namespace CCMS.BL.Services.EditorConfig.Properties.Helpers
{
    /// <summary>Extension methods used in properties.</summary>
    public static class PropertyExtensions
    {
        /// <summary>Changes property type name to snake-case, discarding suffix "property".</summary>
        public static string ToName<T>(this T property) where T : class, IProperty
        {
            if (property == null) throw new ArgumentNullException(nameof(property));
            var name = property.GetType().Name.RemoveSuffix("property");
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(char.ToLowerInvariant(name[0]));
            for (var i = 1; i < name.Length; ++i)
            {
                var value = name[i];
                if (char.IsUpper(value))
                {
                    stringBuilder.Append('_');
                    stringBuilder.Append(char.ToLowerInvariant(value));
                }
                else
                {
                    stringBuilder.Append(value);
                }
            }

            return stringBuilder.ToString();
        }

        /// <summary>Converts enum type into string of values.</summary>
        public static string ToAllowedValue(this Type enumType)
        {
            if (!enumType.IsEnum) throw new ArgumentOutOfRangeException(nameof(enumType));
            return "Enum: " + Enum.GetValues(enumType).OfType<Enum>()
                .Select(s => s.ToString().Replace("_", "-"))
                .Aggregate(new StringBuilder(),
                    (current, next) => current.Append(current.Length == 0 ? "\"" : "\", \"").Append(next)) + "\"";
        }

        /// <summary>Converts enum type into list of values in string.</summary>
        public static IEnumerable<string> ToStringArray(this Type enumType)
        {
            if (!enumType.IsEnum) throw new ArgumentOutOfRangeException(nameof(enumType));
            return Enum.GetValues(enumType).OfType<Enum>()
                .Select(s => s.ToString().Replace("_", "-"));
        }

        private static string RemoveSuffix(this string text, string suffix)
        {
            return text.ToLower().EndsWith(suffix.ToLower()) ? text[..^suffix.Length] : text;
        }
    }
}