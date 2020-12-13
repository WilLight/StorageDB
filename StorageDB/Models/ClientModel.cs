using System;

namespace StorageDB.Models
{
    public class ClientModel : IModelIndexed
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}