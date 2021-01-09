using System;
using System.Collections.Generic;
using StorageDB.Models;

namespace StorageDB.Data
{
    public interface ILiteDbCustomerRepository : ILiteDbIndexedRepository<CustomerModel>
    {
         
    }
}