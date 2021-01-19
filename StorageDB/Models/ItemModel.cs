using System;

namespace StorageDB.Models
{
    public class ItemModel : BaseModel
    {
        public string Name { get; set; }
        public float Size { get; set; }
    }
}