using InventoryModels;
using InventoryModels.Dtos;
using System;
using System.Collections.Generic;

namespace InventoryDatabaseLayer
{
    public interface IInventoryDatabaseRepo
    {
        List<Item> ListInventory();
        List<GetItemsForListingWithDateDto> GetItemsForListingLinq(DateTime minDateValue, DateTime maxDateValue);
        List<GetItemsForListingDto> GetItemsForListingFromProcedure(DateTime dateDateValue, DateTime maxDateValue);
        List<GetItemsTotalValueDto> GetItemsTotalValues(bool isActive);
        List<ItemsWithGenresDto> GetItemsWithGenres();
        List<CategoryDto> ListCategoriesAndColors();
        int InsertOrUpdateItem(Item item);
        void InsertOrUpdateItems(List<Item> items);
        void DeleteItem(int id);
        void DeleteItems(List<int> itemIds);

    }

}
