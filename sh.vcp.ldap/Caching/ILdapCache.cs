using System.Collections.Generic;

namespace sh.vcp.ldap.Caching
{
    /// <summary>
    /// A cache to cache ldap queries and access.
    /// </summary>
    internal interface ILdapCache
    {
        /// <summary>
        /// Tries to load an entry from cache.
        /// </summary>
        /// <param name="key">Key of the entry</param>
        /// <param name="value">Out Value</param>
        /// <typeparam name="T">Type of the entry</typeparam>
        /// <returns>Boolean that indicates success.</returns>
        bool TryGetValue<T>(string key, out T value);

        /// <summary>
        /// Save an entry to cache.
        /// </summary>
        /// <param name="key">Key of the entry</param>
        /// <param name="value">Value of the entry</param>
        /// <typeparam name="T">Type of the entry</typeparam>
        void Set<T>(string key, T value);

        /// <summary>
        /// Removes an entry from cache.
        /// </summary>
        /// <param name="key">Key of the entry</param>
        void Remove(string key);

        /// <summary>
        /// Tries to load a search from cache.
        /// </summary>
        /// <param name="baseDn"></param>
        /// <param name="scope"></param>
        /// <param name="filter"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        bool TryGetSearch(string baseDn, int scope, string filter,
            out SearchCacheEntry search);

        /// <summary>
        /// Save a search to cache.
        /// </summary>
        /// <param name="baseDn"></param>
        /// <param name="scope"></param>
        /// <param name="filter"></param>
        /// <param name="select"></param>
        void SetSearch(string baseDn, int scope, string filter, IEnumerable<string> entries);

        /// <summary>
        /// Invalidates a search if entries get updated or added
        /// </summary>
        /// <param name="key"></param>
        void InvalidateSearch(string key);
    }
}