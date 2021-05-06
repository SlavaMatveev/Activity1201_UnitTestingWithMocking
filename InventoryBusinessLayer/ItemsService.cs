using AutoMapper;
using InventoryDatabaseCore;
using InventoryDatabaseLayer;
using InventoryModels;
using InventoryModels.Dtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace InventoryBusinessLayer
{
    public class ItemsService : IItemsService
    {
        private readonly IInventoryDatabaseRepo _dbRepo;
        private readonly IMapper _mapper;

        public ItemsService(InventoryDbContext dbContext, IMapper mapper)
        {
            _dbRepo = new InventoryDatabaseRepo(dbContext, mapper);
            _mapper = mapper;
        }

        public List<GetItemsForListingDto> GetItemsForListingFromProcedure(DateTime minDateValue, DateTime maxDateValue)
        {
            return _dbRepo.GetItemsForListingFromProcedure(minDateValue, maxDateValue);
        }

        public List<GetItemsForListingWithDateDto> GetItemsForListingLinq(DateTime minDateValue, DateTime maxDateValue)
        {
            return _dbRepo.GetItemsForListingLinq(minDateValue, maxDateValue);
        }

        public AllItemsPipeDelimitedStringDto GetItemsPipeDelimitedString(bool isActive)
        {
            var items = ListInventory();
            var sb = new StringBuilder();

            foreach (var item in items)
            {
                if (sb.Length > 0)
                {
                    sb.Append("|");
                }
                sb.Append(item.Name);
            }

            var output = new AllItemsPipeDelimitedStringDto();
            output.AllItems = sb.ToString();
            return output;
        }

        public List<GetItemsTotalValueDto> GetItemsTotalValues(bool isActive)
        {
            return _dbRepo.GetItemsTotalValues(isActive);
        }

        public List<ItemsWithGenresDto> GetItemsWithGenres()
        {
            return _dbRepo.GetItemsWithGenres();
        }

        public List<CategoryDto> ListCategoriesAndColors()
        {
            return _dbRepo.ListCategoriesAndColors();
        }

        public List<ItemDto> ListInventory()
        {
            return _mapper.Map<List<ItemDto>>(_dbRepo.ListInventory());
        }

        public int InsertOrUpdateItem(CreateOrUpdateItemDto item)
        {
            if (item.CategoryId <= 0)
            {
                throw new ArgumentException("Please set the category id before insert or update");
            } 
            return _dbRepo.InsertOrUpdateItem(_mapper.Map<Item>(item));
        }

        public void InsertOrUpdateItems(List<CreateOrUpdateItemDto> items)
        {
            _dbRepo.InsertOrUpdateItems(_mapper.Map<List<Item>>(items));
        }

        public void DeleteItem(int id)
        {
            if (id <= 0) 
            {
                throw new ArgumentException("Please set a valid item id before deleting");
            }
            _dbRepo.DeleteItem(id);
        }

        public void DeleteItems(List<int> itemIds)
        {
            try
            {
                _dbRepo.DeleteItems(itemIds);
            }
            catch (Exception ex)
            {
                //TODO: better logging/not squelching
                Console.WriteLine($"The transaction has failed: {ex.Message}");
            }
        }

    }
}
