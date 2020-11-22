using System;

namespace StorageDB.Models
{
    public class StorageModel : IModelIndexed
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
    }
}