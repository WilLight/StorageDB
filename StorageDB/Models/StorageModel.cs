using System;

namespace StorageDB.Models
{
    public class StorageModel : BaseModel
    {
        public string Name { get; set; }
        public int Capacity { get; set; }
    }
}