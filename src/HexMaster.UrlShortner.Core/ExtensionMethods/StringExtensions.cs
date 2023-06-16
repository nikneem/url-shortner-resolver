using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HexMaster.UrlShortner.Core.ExtensionMethods
{
    public static class StringExtensions
    {

        public static string ToCamelCase(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            var sb = new StringBuilder();
            var words = input.Split(' ');
            foreach (var word in words)
            {
                if (word.Length > 1)
                {
                    sb.Append(word.Substring(0, 1).ToUpper());
                    sb.Append(word.Substring(1).ToLower());
                }
                else
                {
                    sb.Append(word.ToUpper());
                }
            }
            return sb.ToString();
        }


        // This fuction is an extension method that accepts a word as a string, and returns a string with the first character in lowercase.
        public static string LowerCaseFirstCharecter(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }
            var sb = new StringBuilder(input.Substring(0, 1).ToLower());
            sb.Append(input.Substring(1, input.Length - 1));
            return sb.ToString();
            
        }


    }
}
