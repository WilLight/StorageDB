using System;

namespace StorageDB.Models
{
    public class Storage
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
    }
}