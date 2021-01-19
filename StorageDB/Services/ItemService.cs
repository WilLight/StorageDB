using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorageDB.Data;
using StorageDB.Services;
using StorageDB.Models;

namespace StorageDB.Services
{
    public class ItemService : IItemService
    {
        private readonly LiteDbItemRepository _dbItemRepository;

        public ItemService(LiteDbItemRepository dbItemRepository)
        {
            _dbItemRepository = dbItemRepository;
        }

        public IEnumerable<ItemModel> GetAll()
        {
            return _dbItemRepository.FindAll().OrderBy(item => item.Name);
        }

        public ItemModel GetOne(Guid itemId)
        {
            return _dbItemRepository.FindOne(itemId);
        }

        public ItemModel InsertOne(ItemModel item)
        {
            if(GetOne(item.Id) != null)
                return null;
            item.Id = Guid.NewGuid();
            if(_dbItemRepository.Insert(item) != default)
                return item;
            else
                return null;
        }
        public ItemModel UpdateOne(ItemModel item)
        {
            if(_dbItemRepository.UpdateOne(item))
                return item;
            else
                return null;
        }

        public float CountItemsPerCell(Guid itemId)
        {
            var itemSize = GetOne(itemId).Size;
            return (1 / itemSize);
        }
    }
}