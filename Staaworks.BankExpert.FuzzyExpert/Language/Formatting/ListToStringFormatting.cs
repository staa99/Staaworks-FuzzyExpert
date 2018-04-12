using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Staaworks.BankExpert.FuzzyExpert.Language.Formatting
{
    static class ListToStringFormatting
    {
        public static string ToDString<T> (this List<T> list)
        {
            var output =
                "[";

            if (list.Any())
            {
                output += list[0];
            }

            foreach (var item in list.Skip(1))
            {
                output += "; " + item;
            }

            output +=
                "]";

            return output;
        }
    }




    static class DictionaryToStringFormatting
    {
        public static string ToDString<T>(this Dictionary<string, T> map)
        {
            var output =
                "[";

            if (map.Any())
            {
                var p = map.FirstOrDefault();

                if (int.TryParse(p.Key, out int key))
                {
                    output += p.Value;
                }
                else
                {
                    output += String.Format("{0}: {1}", p.Key, p.Value);
                }
            }
            foreach (var pair in map.Skip(1))
            {
                if (int.TryParse(pair.Key, out int key))
                {
                    output += "; " + pair.Value;
                }
                else
                {
                    output += String.Format("; {0}: {1}", pair.Key, pair.Value);
                }
            }

            output +=
                "]";

            return output;
        }
    }
}
