using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using sh.vcp.ldap.Caching;
using sh.vcp.ldap.ChangeTracking;
using sh.vcp.ldap.Exceptions;
using sh.vcp.ldap.Extensions;

namespace sh.vcp.ldap
{
    internal class LdapConnection : Novell.Directory.Ldap.LdapConnection, ILdapConnection
    {
        private readonly string _bindDn;
        private readonly string _bindPassword;
        private readonly LdapConfig _config;
        private readonly ILogger<LdapConnection> _logger;
        private readonly ChangeTrackingDbContext _trackingDbContext;
        private readonly ILdapCache _cache;
        private bool _connected;


        public LdapConnection(LdapConfig config, ILogger<LdapConnection> logger,
            ChangeTrackingDbContext trackingDbContext, ILdapCache cache) {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            this._logger = logger;
            this._trackingDbContext = trackingDbContext;
            this._cache = cache;
        }

        private LdapConnection(LdapConfig config, ILogger<LdapConnection> logger, string dn, string password,
            ChangeTrackingDbContext trackingDbContext,
            IMemoryCache cache) {
            this._logger = logger;
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            this._bindDn = dn ?? throw new ArgumentNullException(nameof(dn));
            this._bindPassword = password ?? throw new ArgumentNullException(nameof(password));
            this._trackingDbContext = trackingDbContext;
        }

        #region SEARCH
        public async Task<TModel> SearchFirst<TModel>(string baseDn, string filter, string objectClass, int scope,
            string[] attributes,
            bool expectUnique, CancellationToken cancellationToken) where TModel : LdapModel, new() {
            if (objectClass == null) throw new ArgumentNullException(nameof(objectClass));
            return await Task.Run(async () => {
                if (!this._connected) this.ConnectIfDisconnected();

                List<TModel> entries = new List<TModel>();
                if (filter == null) {
                    filter = $"{LdapProperties.ObjectClass}={objectClass}";
                }

                if (this._config.UseCache) {
                    SearchCacheEntry search;
                    if (!this._cache.TryGetSearch(baseDn, scope, filter, out search)) {
                        var queue = this.Search(baseDn, scope, filter,
                            attributes,
                            false);
                        while (queue.HasMore()) {
                            var m = new TModel();
                            m.ProvideEntry(queue.Next());
                            if (typeof(ILdapModelWithChildren).IsAssignableFrom(typeof(TModel)))
                                await ((ILdapModelWithChildren) m).LoadChildren(this, cancellationToken);
                            entries.Add(m);
                        }

                        this._cache.SetSearch(baseDn, scope, filter, entries.Select(e => e.Dn));
                    }
                    else {
                        await search.Entries.ForEachAsync(async entry =>
                            entries.Add(await this.Read<TModel>(entry, cancellationToken))
                        );
                    }
                }
                else {
                    var queue = this.Search(baseDn, scope, filter,
                        attributes,
                        false);
                    while (queue.HasMore()) {
                        var m = new TModel();
                        m.ProvideEntry(queue.Next());
                        if (typeof(ILdapModelWithChildren).IsAssignableFrom(typeof(TModel)))
                            await ((ILdapModelWithChildren) m).LoadChildren(this, cancellationToken);
                        entries.Add(m);
                    }
                }

                if (expectUnique && entries.Count > 1) throw new LdapSearchNotUniqueException(filter, entries.Count);

                return entries.FirstOrDefault();
            }, cancellationToken);
        }

        public async Task<ICollection<TModel>> Search<TModel>(string baseDn, string filter, string objectClass,
            int scope, string[] attributes,
            CancellationToken cancellationToken = default) where TModel : LdapModel, new() {
            if (objectClass == null) throw new ArgumentNullException(nameof(objectClass));
            if (!this._connected) this.ConnectIfDisconnected();

            filter = string.IsNullOrEmpty(filter)
                ? $"{LdapProperties.ObjectClass}={objectClass}"
                : $"(&({LdapProperties.ObjectClass}={objectClass})({filter}))";

            ICollection<TModel> entries = new List<TModel>();
            if (this._config.UseCache) {
                SearchCacheEntry search;
                if (!this._cache.TryGetSearch(baseDn, scope, filter, out search)) {
                    var queue = this.Search(baseDn, scope, filter, attributes,
                        false);
                    while (queue.HasMore()) {
                        var m = new TModel();
                        m.ProvideEntry(queue.Next());
                        if (typeof(ILdapModelWithChildren).IsAssignableFrom(typeof(TModel)))
                            await ((ILdapModelWithChildren) m).LoadChildren(this, cancellationToken);
                        entries.Add(m);
                    }

                    this._cache.SetSearch(baseDn, scope, filter, entries.Select(e => e.Dn));
                }
                else {
                    await search.Entries.ForEachAsync(async entry =>
                        entries.Add(await this.Read<TModel>(entry, cancellationToken))
                    );
                }
            }
            else {
                var queue = this.Search(baseDn, scope, filter, attributes,
                    false);
                while (queue.HasMore()) {
                    var m = new TModel();
                    m.ProvideEntry(queue.Next());
                    if (typeof(ILdapModelWithChildren).IsAssignableFrom(typeof(TModel)))
                        await ((ILdapModelWithChildren) m).LoadChildren(this, cancellationToken);
                    entries.Add(m);
                }
            }

            return entries;
        }
        
        public async Task<ICollection<TModel>> SearchSafe<TModel>(string baseDn, string filter, string objectClass,
            int scope,
            CancellationToken cancellationToken) where TModel : LdapModel, new() {
            if (objectClass == null) throw new ArgumentNullException(nameof(objectClass));
            if (!this._connected) this.ConnectIfDisconnected();

            filter = string.IsNullOrEmpty(filter)
                ? $"{LdapProperties.ObjectClass}={objectClass}"
                : $"(&({LdapProperties.ObjectClass}={objectClass})({filter}))";

            ICollection<TModel> entries = new List<TModel>();
            if (this._config.UseCache) {
                if (!this._cache.TryGetSearch(baseDn, scope, filter, out var search)) {
                    var queue = this.Search(baseDn, scope, filter, new [] { LdapProperties.ObjectClass }, false);
                    while (queue.HasMore()) {
                        var m = new TModel();
                        m.ProvideEntry(queue.Next());
                        entries.Add(m);
                    }

                    this._cache.SetSearch(baseDn, scope, filter, entries.Select(e => e.Dn));
                }
                else {
                    await search.Entries.ForEachAsync(async entry =>
                        entries.Add(await this.Read<TModel>(entry, cancellationToken))
                    );
                }
            }
            else {
                var queue = this.Search(baseDn, scope, filter, new [] { LdapProperties.ObjectClass },
                    false);
                while (queue.HasMore()) {
                    var m = new TModel();
                    m.ProvideEntry(queue.Next());
                    entries.Add(m);
                }
            }

            return entries;
        }
        
        #endregion SEARCH

        #region READ
        public async Task<TModel> ReadSafe<TModel>(string dn, CancellationToken cancellationToken)
            where TModel : LdapModel, new()
        {
            try {
                this.ConnectIfDisconnected();

                if (this._config.UseCache && !this._cache.TryGetValue(dn, out TModel model)) {
                    var entry = this.Read(dn);
                    model = new TModel();
                    model.ProvideEntry(entry);
                    this._cache.Set(dn, model);
                }
                else {
                    var entry = this.Read(dn);
                    model = new TModel();
                    model.ProvideEntry(entry);
                }

                if (typeof(ILdapModelWithChildren).IsAssignableFrom(typeof(TModel))) {
                    await ((ILdapModelWithChildren) model).LoadChildren(this, cancellationToken);
                }

                return model;
            }
            catch (LdapException ex) when(ex.ResultCode == 34) {
                return null;
            }
        }


        public async Task<TModel> Read<TModel>(string dn, CancellationToken cancellationToken = default)
            where TModel : LdapModel, new() {
            try {
                if (!this._connected) this.ConnectIfDisconnected();

                TModel model;
                if (this._config.UseCache) {
                    if (!this._cache.TryGetValue(dn, out model)) {
                        var entry = this.Read(dn);
                        model = new TModel();
                        model.ProvideEntry(entry);
                        this._cache.Set(dn, model);
                    }
                }
                else {
                    var entry = this.Read(dn);
                    model = new TModel();
                    model.ProvideEntry(entry);
                }

                if (typeof(ILdapModelWithChildren).IsAssignableFrom(typeof(TModel)))
                    await ((ILdapModelWithChildren) model).LoadChildren(this, cancellationToken);
                return model;
            }
            catch (LdapException ex) {
                if (ex.ResultCode == 34) {
                    var ldapEx = new LdapDnInvalidException(dn);
                    this._logger.LogError(ldapEx, LdapLogCodes.LdapReadError);
                    throw ldapEx;
                }

                this._logger.LogError(ex, $"{LdapLogCodes.LdapReadError} ({dn})");
                return null;
            }
        }
        
        #endregion READ

        public Task<bool> Bind(string dn, string password, CancellationToken cancellationToken) {
            return Task.Run(() => {
                try {
                    using (var con =
                        new LdapConnection(this._config, this._logger, this._trackingDbContext, this._cache)) {
                        con.ConnectIfDisconnected();
                        return con.Bound;
                    }
                }
                catch (LdapException ex) {
                    this._logger.LogError(ex, LdapLogCodes.LdapBindError);
                    this._connected = false;
                }

                return false;
            }, cancellationToken);
        }

        public async Task<TModel> Add<TModel>(TModel model, string changedBy, CancellationToken cancellationToken)
            where TModel : LdapModel {
            try {
                if (!this._connected) this.ConnectIfDisconnected();
                var entry = model.ToEntry();
                this.Add(entry);
                if (this._config.LogChanges) {
                    IEnumerable<Change> changes = entry.getAttributeSet().ToChangesAdd(model.Dn, changedBy);
                    await this._trackingDbContext.AddRangeAsync(changes, cancellationToken);
                    await this._trackingDbContext.SaveChangesAsync(cancellationToken);
                }

                if (this._config.UseCache) {
                    this._cache.InvalidateSearch(model.Dn);
                }

                return model;
            }
            catch (LdapException ex) {
                switch (ex.ResultCode) {
                    case 68: {
                        throw new Exception("Entry already exists");
                    }
                    default:
                        this._logger.LogError(ex, LdapLogCodes.LdapAddError);
                        throw new Exception($"Adding {model.Dn} failed");
                }
            }
        }

        public async Task<TModel> AddChildren<TModel>(TModel model, string changedBy, CancellationToken cancellationToken)
            where TModel : LdapModel, ILdapModelWithChildren {
            try {
                foreach (var ldapModel in model.GetChildren()) await this.Add(ldapModel, changedBy, cancellationToken);
                return model;
            }
            catch (LdapException ex) {
                this._logger.LogError(ex, LdapLogCodes.LdapAddChildrenError);
                return null;
            }
        }

        public Task<bool> Update<TModel>(TModel model, string changedBy, CancellationToken cancellationToken)
            where TModel : LdapModel, new() {
            return Task.Run(() => this.Update<TModel>(model.Dn, model.GetModifications(), changedBy, cancellationToken),
                cancellationToken);
        }

        public async Task<bool> Update<TModel>(string dn, LdapModification[] ldapModifications, string changedBy,
            CancellationToken cancellationToken = default) where TModel : LdapModel, new() {
            try {
                if (ldapModifications.Length <= 0) return true;
                if (!this._connected) this.ConnectIfDisconnected();

                this.Modify(dn, ldapModifications);

                if (this._config.LogChanges) {
                    var oldObject = await this.Read<TModel>(dn, cancellationToken);
                    IEnumerable<Change> changes = ldapModifications.ToChangesModify(dn, changedBy, oldObject.ObjectClasses);
                    await this._trackingDbContext.AddRangeAsync(changes, cancellationToken);
                    await this._trackingDbContext.SaveChangesAsync(cancellationToken);
                }

                if (this._config.UseCache) {
                    this._cache.Remove(dn);
                }

                return true;
            }
            catch (Exception ex) {
                this._logger.LogError(ex, LdapLogCodes.LdapModifyError);
                return false;
            }
        }
        
        #region InternalMethods

        private void ConnectIfDisconnected() {
            if (this._connected) return;
            try {
                this.Connect(this._config.Hostname, this._config.Port);
                this.Bind(this._bindDn ?? this._config.AdminUserDn,
                    this._bindPassword ?? this._config.AdminUserPassword);
                this._connected = true;
            }
            catch (LdapException ex) {
                this._logger.LogError(ex, LdapLogCodes.LdapConnectError);
                this._connected = false;
            }
        }
        
        #endregion InternalMethods
        
    }
}