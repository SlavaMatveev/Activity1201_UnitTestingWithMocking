using AutoMapper;
using AutoMapper.QueryableExtensions;
using InventoryDatabaseCore;
using InventoryModels;
using InventoryModels.Dtos;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Transactions;

namespace InventoryDatabaseLayer
{
    public class InventoryDatabaseRepo : IInventoryDatabaseRepo
    {
        private readonly InventoryDbContext _context;
        private readonly IMapper _mapper;

        public InventoryDatabaseRepo(InventoryDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<GetItemsForListingDto> GetItemsForListingFromProcedure(DateTime dateDateValue, DateTime maxDateValue)
        {
            var minDateParam = new SqlParameter("minDate", dateDateValue);
            var maxDateParam = new SqlParameter("maxDate", maxDateValue);

            return _context.ItemsForListing
                        .FromSqlRaw("EXECUTE dbo.GetItemsForListing @minDate, @maxDate"
                                        , minDateParam, maxDateParam)
                        .ToList();
        }

        public List<GetItemsForListingWithDateDto> GetItemsForListingLinq(DateTime minDateValue, DateTime maxDateValue)
        {
            return _context.Items.Include(x => x.Category).AsEnumerable()
                            .Select(x => new GetItemsForListingWithDateDto
            {
                CreatedDate = x.CreatedDate,
                CategoryName = x.Category.Name,
                Description = x.Description,
                IsActive = x.IsActive,
                IsDeleted = x.IsDeleted,
                Name = x.Name,
                Notes = x.Notes
            }).Where(x => x.CreatedDate >= minDateValue && x.CreatedDate <= maxDateValue)
                    .AsQueryable().OrderBy(y => y.CategoryName).ThenBy(z => z.Name).ToList();
        }

        public List<GetItemsTotalValueDto> GetItemsTotalValues(bool isActive)
        {
            var isActiveParm = new SqlParameter("IsActive", 1);

            return _context.GetItemsTotalValues
                           .FromSqlRaw("SELECT * from [dbo].[GetItemsTotalValue] (@IsActive)", isActiveParm)
                           .ToList();
        }

        public List<CategoryDto> ListCategoriesAndColors()
        {
            return _context.Categories
                            .Include(x => x.CategoryColor)
                            .ProjectTo<CategoryDto>(_mapper.ConfigurationProvider).ToList();
        }

        public List<Item> ListInventory()
        {
            return _context.Items.Include(x => x.Category)
                       .AsEnumerable()
                       .Where(x => !x.IsDeleted)
                       .OrderBy(x => x.Name).ToList();
        }

        public List<ItemsWithGenresDto> GetItemsWithGenres()
        {
            return _context.ItemsWithGenres.ToList();
        }

        public int InsertOrUpdateItem(Item item)
        {
            if (item.Id > 0)
            {
                return UpdateItem(item);
            }
            return CreateItem(item);
        }

        private int CreateItem(Item item)
        {
            _context.Items.Add(item);
            _context.SaveChanges();
            var newItem = _context.Items.ToList()
                            .FirstOrDefault(x => x.Name.ToLower()
                            .Equals(item.Name.ToLower()));

            if (newItem == null) throw new Exception("Could not Create the item as expected");

            return newItem.Id;
        }

        private int UpdateItem(Item item)
        {
            var dbItem = _context.Items.FirstOrDefault(x => x.Id == item.Id);
            dbItem.CategoryId = item.CategoryId;
            dbItem.CurrentOrFinalPrice = item.CurrentOrFinalPrice;
            dbItem.Description = item.Description;
            dbItem.IsActive = item.IsActive;
            dbItem.IsDeleted = item.IsDeleted;
            dbItem.IsOnSale = item.IsOnSale;
            dbItem.Name = item.Name;
            dbItem.Notes = item.Notes;
            dbItem.PurchasedDate = item.PurchasedDate;
            dbItem.PurchasePrice = item.PurchasePrice;
            dbItem.Quantity = item.Quantity;
            dbItem.SoldDate = item.SoldDate;
            _context.SaveChanges();
            return item.Id;
        }

        public void InsertOrUpdateItems(List<Item> items)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required
                        , new TransactionOptions 
                            { IsolationLevel = IsolationLevel.ReadUncommitted }))
            {
                try
                {
                    foreach (var item in items)
                    {
                        var success = InsertOrUpdateItem(item) > 0;
                        if (!success) throw new Exception($"Error saving the item {item.Name}");
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    //log it:
                    Debug.WriteLine(ex.ToString());
                    throw ex;
                }
            }
        }

        public void DeleteItem(int id)
        {
            var item = _context.Items.FirstOrDefault(x => x.Id == id);
            if (item == null) return;
            item.IsDeleted = true;
            _context.SaveChanges();
        }

        public void DeleteItems(List<int> itemIds)
        {
            using (var scope = new TransactionScope(TransactionScopeOption.Required
                        , new TransactionOptions
                        { IsolationLevel = IsolationLevel.ReadUncommitted }))
            { 
                try
                {
                    foreach (var itemId in itemIds)
                    {
                        DeleteItem(itemId);
                    }

                    scope.Complete();
                }
                catch (Exception ex)
                {
                    //log it:
                    Debug.WriteLine(ex.ToString());
                    throw ex;  //make sure it is known that the transaction failed
                }
            }
        }

    }
}
