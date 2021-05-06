using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace InventoryModels
{
    public class FullAuditModel : IIdentityModel, IAuditedModel, IActivatableModel, ISoftDeletable
    {
        [Key]
        public int Id { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? LastModifiedUserId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsActive { get; set; }
        [Required]
        [DefaultValue(false)]
        public bool IsDeleted { get; set; }
    }
}
