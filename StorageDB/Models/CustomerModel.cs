using System;

namespace StorageDB.Models
{
    public class CustomerModel : IModelIndexed
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}