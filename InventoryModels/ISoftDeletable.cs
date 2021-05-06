namespace InventoryModels
{
    public interface ISoftDeletable
    {
        bool IsDeleted { get; set; }
    }
}
