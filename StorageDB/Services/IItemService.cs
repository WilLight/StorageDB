using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageDB.Models;
using Microsoft.AspNetCore.Mvc;

namespace StorageDB.Services
{
    public interface IItemService
    {
         IEnumerable<ItemModel> GetAll();
         ItemModel GetOne(Guid itemId);
         ItemModel InsertOne(ItemModel item);
         ItemModel UpdateOne(ItemModel item);
    }
}