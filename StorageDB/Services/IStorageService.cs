using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageDB.Models;
using Microsoft.AspNetCore.Mvc;

namespace StorageDB.Services
{
    public interface IStorageService
    {
        IEnumerable<StorageModel> GetAll();
        StorageModel GetOne(Guid storageId);
        StorageModel InsertOne(StorageModel storage);
        StorageModel UpdateOne(StorageModel storage);
    }
}