using System;

namespace StorageDB.Models
{
    public class Item
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Size { get; set; }
    }
}