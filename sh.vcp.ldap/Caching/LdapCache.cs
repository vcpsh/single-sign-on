using System;
using System.Collections.Generic;
using Microsoft.Extensions.Caching.Memory;

namespace sh.vcp.ldap.Caching
{
    internal class LdapCache : ILdapCache
    {
        private readonly MemoryCache _cache;
        private readonly Dictionary<string, string> _searchEntries = new Dictionary<string, string>();

        private readonly MemoryCacheEntryOptions _defaultOptions =
            new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30));

        public LdapCache() {
            this._cache = new MemoryCache(new MemoryCacheOptions());
        }

        public bool TryGetValue<T>(string key, out T value) {
            return this._cache.TryGetValue(key, out value);
        }
 
        public void Set<T>(string key, T value) {
            this._cache.Set(key, value, this._defaultOptions);
        }

        public void Remove(string key) {
            this.InvalidateSearchInternal(key);
            this._cache.Remove(key);
        }

        public bool TryGetSearch(string baseDn, int scope, string filter,
            out SearchCacheEntry search) {
            var key = GenerateSearchKey(baseDn, scope, filter);
            return this.TryGetValue(key, out search);
        }

        public void SetSearch(string baseDn, int scope, string filter,
            IEnumerable<string> entries) {
            var key = GenerateSearchKey(baseDn, scope, filter);
            if (!this._searchEntries.ContainsKey(key)) {
                this._searchEntries.Add(key, baseDn);
            }
            else {
                this._searchEntries[key] = baseDn;
            }
            this.Set(key, new SearchCacheEntry {Entries = entries});
        }

        public void InvalidateSearch(string key) {
            this.InvalidateSearchInternal(key);
        }

        private static string GenerateSearchKey(string baseDn, int scope, string filter) {
            return $"{baseDn}_{scope}_{filter}";
        }

        private void InvalidateSearchInternal(string key) {
            this._searchEntries.Remove(key);
        }
    }
}
