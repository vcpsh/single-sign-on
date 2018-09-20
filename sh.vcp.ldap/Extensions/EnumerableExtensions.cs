using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace sh.vcp.ldap.Extensions
{
    public static class EnumerableExtensions
    {
        public static async Task ForEachAsync<T>(this IEnumerable<T> col, Func<T, Task> method) {
            foreach (var entry in col) await method(entry);
        }

        public static async Task<bool> ExistsAsync<T>(this IEnumerable<T> col, AsyncPredicate<T> predicate) {
            foreach (var entry in col)
                if (await predicate(entry))
                    return true;
            return false;
        }

        public delegate Task<bool> AsyncPredicate<in T>(T obj);
    }
}