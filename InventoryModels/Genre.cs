using System.ComponentModel.DataAnnotations;

namespace InventoryModels
{
    public class Genre : FullAuditModel
    {
        [Required]
        [StringLength(InventoryModelsConstants.MAX_NAME_LENGTH)]
        public string Name { get; set; }
    }
}
