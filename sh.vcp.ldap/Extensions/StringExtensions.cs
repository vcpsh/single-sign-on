using System;

namespace sh.vcp.ldap.Extensions
{
    public static class StringExtensions
    {
        public static string Replace(this string source, string oldValue, string newValue, StringComparison comparison)
        {
            while (true) {
                var index = source.IndexOf(oldValue, comparison);
                if (index >= 0) {
                    source = source.Remove(index, oldValue.Length);
                    source = source.Insert(index, newValue);
                }

                if (source.IndexOf(oldValue, comparison) != -1) {
                    continue;
                }

                return source;
            }
        }
    }
}