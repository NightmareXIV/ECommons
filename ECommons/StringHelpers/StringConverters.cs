using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ECommons.StringHelpers
{
    public static class StringConverters
    {
        public static string ToBase64(this string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(bytes);
        }

        public static string FromBase64(this string value)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(value);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
