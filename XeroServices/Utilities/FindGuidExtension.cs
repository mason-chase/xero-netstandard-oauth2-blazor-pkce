using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace XeroServices.Utilities
{
    public static class FindGuidExtension
    {
        public static Guid? FindGuid(this string value)
        {
            var matched = Regex.Match(value, @"(?<guid>[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12})", RegexOptions.IgnoreCase);
            try
            {
                return Guid.Parse(matched.Groups["guid"].Value);
            }
            catch (Exception ex) when (ex is ArgumentNullException || ex is FormatException)
            {
                return null;
            }
        }
    }
}
