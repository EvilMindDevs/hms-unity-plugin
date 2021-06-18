using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace HmsPlugin
{
    public static class StringExtensions
    {
        // Removes substring at the end if present
        public static string RemoveEndSubStr(this string inputText, string toRemove)
        {
            if (inputText.EndsWith(toRemove))
            {
                return inputText.Substring(0, inputText.LastIndexOf(toRemove, StringComparison.InvariantCulture));
            }

            return inputText;
        }


        public static string RemoveStartSubStr(this string inputText, string toRemove)
        {
            if (inputText.StartsWith(toRemove))
            {
                return inputText.Substring(toRemove.Length, inputText.Length - toRemove.Length);
            }

            return inputText;
        }

        public static bool StartsWithAny(this string text, params string[] options)
        {
            foreach (var option in options)
            {
                if (text.StartsWith(option))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool EndsWithAny(this string text, params string[] options)
        {
            foreach (var option in options)
            {
                if (text.EndsWith(option))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsNullOrEmpty(this string text)
        {
            return string.IsNullOrEmpty(text);
        }

        // Clear to be able to reuse the builder without reallocations. (without new .Net versions)
        public static void Clear(this StringBuilder value)
        {
            value.Length = 0;
        }

        public static string PreprocessValue(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            value.Trim(' ');
            value.Trim('\n');

            return value;
        }

        public static string RemoveSpecialCharacters(this string value)
        {
            if (string.IsNullOrEmpty(value)) return value;

            value.Replace(" ", "");
            value.Trim('\n');
            value = Regex.Replace(value, @"[^0-9a-zA-Z_]+", "");

            return value;
        }

        public static string RemoveAfter(this string value, char toRemove)
        {
            int index = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == toRemove)
                {
                    index = i;
                    break;
                }
            }
            index = index == 0 ? value.Length : index;
            return value.Substring(0, index);
        }

        public static string ToCamelCase(this string s)
        {
            var x = s.Replace("_", "");
            if (x.Length == 0) return "null";
            x = Regex.Replace(x, "([A-Z])([A-Z]+)($|[A-Z])",
                m => m.Groups[1].Value + m.Groups[2].Value.ToLower() + m.Groups[3].Value);
            return char.ToLower(x[0]) + x.Substring(1);
        }

        public static string ToPascalCase(this string s)
        {
            var x = ToCamelCase(s);
            return char.ToUpper(x[0]) + x.Substring(1);
        }
    }
}
