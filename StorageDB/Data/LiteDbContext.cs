using LiteDB;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StorageDB.Data
{
    public class LiteDbContext : ILiteDbContext
    {
        public LiteDatabase Database { get; }

        public LiteDbContext(IOptions<LiteDbOptions> options)
        {
            Database = new LiteDatabase(options.Value.DatabaseLocation);

            /* Do the upgrading and migrations */
            /* Summary: New or removed fields are handled gracefully. 
             * Changing datatype of existing field can be converted in code by (e.g.) using db.Engine.UserVersion to track your db versions. 
             * When renaming class fields, use BsonField attribute to differentiate between storage name and class field name.
             */

            /* Example */
            /*
            if(db.Engine.UserVersion == 0)
            {
                foreach(var doc in db.Engine.Find("MyCol"))
                {
                    doc["NewCol"] = Convert.ToInt32(doc["OldCol"].AsString);
                    db.Engine.Update("MyCol", doc);
                }
                db.Engine.UserVersion = 1;
            } 
             */
        }
    }
}
