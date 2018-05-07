using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using sh.vcp.ldap.Exceptions;

namespace sh.vcp.ldap
{
    internal class LdapConnection : Novell.Directory.Ldap.LdapConnection, ILdapConnection
    {
        private readonly string _bindDn;
        private readonly string _bindPassword;
        private bool _connected;
        private readonly LdapConfig _config;
        private readonly ILogger<LdapConnection> _logger;

        public LdapConnection(LdapConfig config, ILogger<LdapConnection> logger)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            this._logger = logger;
        }

        private LdapConnection(LdapConfig config, ILogger<LdapConnection> logger, string dn, string password)
        {
            this._logger = logger;
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            this._bindDn = dn ?? throw new ArgumentNullException(nameof(dn));
            this._bindPassword = password ?? throw new ArgumentNullException(nameof(password));
        }

        private void Connect()
        {
            if (this._connected) return;
            try
            {
                this.Connect(this._config.Hostname, this._config.Port);
                this.Bind(this._bindDn ?? this._config.AdminUserDn,
                    this._bindPassword ?? this._config.AdminUserPassword);
                this._connected = true;
            }
            catch (LdapException ex)
            {
                this._logger.LogError(ex, LdapLogCodes.LdapConnectError);
                this._connected = false;
            }
        }

        public async Task<TModel> SearchFirst<TModel>(string baseDn, string filter, string objectClass, int scope, string[] attributes,
            bool expectUnique, CancellationToken cancellationToken) where TModel : LdapModel, new()
        {
            if (objectClass == null) throw new ArgumentNullException(nameof(objectClass));
            return await Task.Run(async () =>
            {
                if (!this._connected)
                {
                    this.Connect();
                }

                var queue = this.Search(baseDn, scope, filter ?? $"{LdapProperties.ObjectClass}={objectClass}", attributes,
                    false);
                List<TModel> entries = new List<TModel>();
                while (queue.HasMore())
                {
                    var m = new TModel();
                    m.ProvideEntry(queue.Next());
                    if (typeof(ILdapModelWithChildren).IsAssignableFrom(typeof(TModel)))
                    {
                        await ((ILdapModelWithChildren) m).LoadChildren(this, cancellationToken);
                    }
                    entries.Add(m);
                }

                if (expectUnique && entries.Count > 1)
                {
                    throw new LdapSearchNotUniqueException(filter, queue.Count);
                }

                return entries.FirstOrDefault();
            }, cancellationToken);
        }

        public async Task<ICollection<TModel>> Search<TModel>(string baseDn, string filter, string objectClass, int scope, string[] attributes,
            CancellationToken cancellationToken = default) where TModel : LdapModel, new()
        {
                if (objectClass == null) throw new ArgumentNullException(nameof(objectClass));
                return await Task.Run(async () =>
                {
                    if (!this._connected)
                    {
                        this.Connect();
                    }

                    filter = string.IsNullOrEmpty(filter)
                        ? $"{LdapProperties.ObjectClass}={objectClass}"
                        : $"(&({LdapProperties.ObjectClass}={objectClass})({filter}))";
                    var queue = this.Search(baseDn, scope, filter, attributes,
                        false);
                    ICollection<TModel> entries = new List<TModel>();
                    while (queue.HasMore())
                    {
                        var m = new TModel();
                        m.ProvideEntry(queue.Next());
                        if (typeof(ILdapModelWithChildren).IsAssignableFrom(typeof(TModel)))
                        {
                            await ((ILdapModelWithChildren) m).LoadChildren(this, cancellationToken);
                        }
                        entries.Add(m);
                    }

                    return entries;
                }, cancellationToken);
        }

        public Task<TModel> Read<TModel>(string dn, CancellationToken cancellationToken = default) where TModel : LdapModel, new()
        {
            return Task.Run(async () =>
            {
                try
                {
                    if (!this._connected)
                    {
                        this.Connect();
                    }
    
                    var entry = this.Read(dn);
                    var m = new TModel();
                    m.ProvideEntry(entry);
                    if (typeof(ILdapModelWithChildren).IsAssignableFrom(typeof(TModel)))
                    {
                        await ((ILdapModelWithChildren) m).LoadChildren(this, cancellationToken);
                    }
                    return m;
                }
                catch (LdapException ex)
                {
                    if (ex.ResultCode == 34)
                    {
                        var ldapEx = new LdapDnInvalidException(dn);
                        this._logger.LogError(ldapEx, LdapLogCodes.LdapReadError);
                        throw ldapEx;
                    }
                    this._logger.LogError(ex, LdapLogCodes.LdapReadError);
                    return null;
                }
            }, cancellationToken);
        }

        public Task<bool> Bind(string dn, string password, CancellationToken cancellationToken)
        {
            return Task.Run(() =>
            {
                try
                {
                    using (var con = new LdapConnection(this._config, this._logger, dn, password))
                    {
                        con.Connect();
                        return con.Bound;
                    }
                }
                catch (LdapException ex)
                {
                    this._logger.LogError(ex, LdapLogCodes.LdapBindError);
                    this._connected = false;
                }

                return false;
            }, cancellationToken);
        }
    }
}