using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StorageDB.Models;
using Microsoft.AspNetCore.Mvc;

namespace StorageDB.Services
{
    public interface ICustomerService
    {
         IEnumerable<CustomerModel> GetAll();
         CustomerModel GetOne(Guid itemId);
         CustomerModel InsertOne(CustomerModel item);
         CustomerModel UpdateOne(CustomerModel item);
    }
}