using System.Collections.Generic;

namespace sh.vcp.ldap.Caching
{
    internal class SearchCacheEntry
    {
        internal IEnumerable<string> Entries { get; set; }
    }
}