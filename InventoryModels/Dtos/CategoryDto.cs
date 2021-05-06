namespace InventoryModels.Dtos
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Category { get; set; }
        public CategoryColorDto CategoryColor { get; set; }
    }
}
