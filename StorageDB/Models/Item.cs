using System;

namespace StorageDB.Models
{
    public class ItemModel : IModelIndexed
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public float Size { get; set; }
    }
}