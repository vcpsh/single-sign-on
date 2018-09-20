using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Caching.Memory;

namespace sh.vcp.ldap.Caching
{
    internal class LdapCache : ILdapCache
    {
        private readonly MemoryCache _cache;
        private Dictionary<string, string> _searchEntries = new Dictionary<string, string>();

        private readonly MemoryCacheEntryOptions _defaultOptions =
            new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(30));

        public LdapCache() {
            this._cache = new MemoryCache(new MemoryCacheOptions { });
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

        public bool TryGetSearch(string baseDn, int scope, string filter, IEnumerable<string> attributes,
            out SearchCacheEntry search) {
            var key = LdapCache.GenerateSearchKey(baseDn, scope, filter, attributes);
            return this.TryGetValue(key, out search);
        }

        public void SetSearch(string baseDn, int scope, string filter, string[] attributes, IEnumerable<string> entries) {
            var key = LdapCache.GenerateSearchKey(baseDn, scope, filter, attributes);
            this.Set(key, new SearchCacheEntry {Entries = entries});
            this._searchEntries.Add(key, baseDn);
        }

        public void InvalidateSearch(string key) {
            this.InvalidateSearchInternal(key);
        }

        private static string GenerateSearchKey(string baseDn, int scope, string filter, IEnumerable<string> attributes) {
            return $"{baseDn}_{scope}_{filter}_{attributes.Aggregate("", (s, s1) => $"{s}_{s1}")}";
        }

        private void InvalidateSearchInternal(string key) {
            this._searchEntries = this._searchEntries.Where(kvp => {
                if (!key.Contains(kvp.Value)) {
                    return true;
                }
                this.Remove(kvp.Value);
                return false;

            }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
    }
}