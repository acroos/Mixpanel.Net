using System;
using System.Collections.Generic;

namespace Mixpanel.NET.PCL.Converters
{
    internal static class PropertyNameConverter
    {
        /// <summary>
        /// Converts a property in camel or pascal case to a string with spaces
        /// </summary>
        /// <returns>The spaced string.</returns>
        /// <param name="property">The property</param>
        public static string ToSpacedString(string property)
        {
            property = property.Replace ("_", "");
            var sb = new System.Text.StringBuilder ();
            for (int i = 0; i < property.Length; i++)
            {
                var letter = property [i];
                if (char.IsNumber(letter))
                {
                    sb.Append (" ");
                }

                if (char.IsUpper(letter) && i != 0)
                {
                    sb.Append (" ");
                }

                if (i == 0)
                {
                    letter = char.ToUpper (letter);
                }

                sb.Append (letter);
            }
            return sb.ToString ();
        }
    }
}
