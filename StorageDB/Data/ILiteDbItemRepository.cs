using StorageDB.Models;

namespace StorageDB.Data
{
    public interface ILiteDbItemRepository : ILiteDbIndexedRepository<ItemModel>
    {
    }
}