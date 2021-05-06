using System;

namespace InventoryModels.Dtos
{
    public class GetItemsForListingWithDateDto : GetItemsForListingDto
    {
        public DateTime CreatedDate { get; set; }
    }
}
