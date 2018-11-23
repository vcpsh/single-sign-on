using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sh.vcp.ldap.ChangeTracking
{
    public class Change
    {
        /// <summary>
        /// Guid of the object.
        /// </summary>
        [Key]
        [Column("guid")]
        public Guid Guid { get; set; }

        /// <summary>
        /// Concurrency token to prevent parallel edits.
        /// </summary>
        [Column("concurrency_token")]
        [Timestamp]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime ConcurrencyToken { get; set; }

        /// <summary>
        /// Creation timestamp. 
        /// </summary>
        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Domain name of the change.
        /// </summary>
        [Column("dn")]
        public string Dn { get; set; }

        [Column("type")]
        public TypeEnum Type { get; set; }

        [Column("object_class")]
        public string ObjectClass { get; set; }

        [Column("property")]
        public string Property { get; set; }

        [Column("new_value")]
        public string NewValue { get; set; }
        
        [Column("changed_by")]
        public string ChangedBy { get; set; }
        
        [Column("change_context")]
        public Guid ChangeContext { get; set; }

        public enum TypeEnum
        {
            Modified,
            Created,
            CreatedAttribute,
            Removed,
            RemovedAttribute,
        }
    }
}