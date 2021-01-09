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
    public class StorageService : IStorageService
    {
        private readonly ILiteDbStorageRepository _dbStorageRepository;

        public StorageService(ILiteDbStorageRepository dbStorageRepository)
        {
            _dbStorageRepository = dbStorageRepository;
        }

        public IEnumerable<StorageModel> GetAll()
        {
            return _dbStorageRepository.FindAll();
        }

        public StorageModel GetOne(Guid storageId)
        {
            return _dbStorageRepository.FindOne(storageId);
        }

        public StorageModel InsertOne(StorageModel storage)
        {
            storage.Id = Guid.NewGuid();
            if(_dbStorageRepository.Insert(storage) != default)
                return storage;
            else
                return null;
        }

        public StorageModel UpdateOne(StorageModel storage)
        {
            if(_dbStorageRepository.UpdateOne(storage))
                return storage;
            else
                return null;
        }
    }
}