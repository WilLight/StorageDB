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
    public class CustomerService : ICustomerService
    {
        private readonly LiteDbCustomerRepository _dbCustomerRepository;

        public CustomerService(LiteDbCustomerRepository dbCustomerRepository)
        {
            _dbCustomerRepository = dbCustomerRepository;
        }

        public IEnumerable<CustomerModel> GetAll()
        {
            return _dbCustomerRepository.FindAll().OrderBy(item => item.Name);
        }

        public CustomerModel GetOne(Guid itemId)
        {
            return _dbCustomerRepository.FindOne(itemId);
        }

        public CustomerModel InsertOne(CustomerModel item)
        {
            if(GetOne(item.Id) != null)
                return null;
            item.Id = Guid.NewGuid();
            if(_dbCustomerRepository.Insert(item) != default)
                return item;
            else
                return null;
        }
        public CustomerModel UpdateOne(CustomerModel item)
        {
            if(_dbCustomerRepository.UpdateOne(item))
                return item;
            else
                return null;
        }
    }
}