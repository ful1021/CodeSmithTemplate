using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SchemaExplorer;

namespace CodeSmithJst
{
    public class JstName
    {
        public static JstName Init(CommonCode comm)
        {
            TableSchema table = (TableSchema)comm.GetProperty("Table");

            var dbName = table.Database.Name;
            var noSplitDbName = CommonCode.GetNoSplitName(dbName);
            var isSplitDb = dbName != noSplitDbName;

            var tableName = table.Name;
            var noSplitTableName = CommonCode.GetNoSplitName(tableName);
            var isSplitTable = tableName != noSplitTableName;

            return new JstName
            {
                IsSplitDb = isSplitDb.ToString().ToLower(),
                IsSplitTable = isSplitTable.ToString().ToLower(),
                ModelName = noSplitTableName,
                RawDbName = isSplitDb ? noSplitDbName + "_" : noSplitDbName,
                RawTableName = isSplitTable ? noSplitTableName + "_" : noSplitTableName,
                CacheRegion = noSplitDbName + noSplitTableName
            };
        }

        public string RawDbName { get; set; }
        public string RawTableName { get; set; }
        public string CacheRegion { get; set; }
        public string ModelName { get; set; }
        public string IsSplitDb { get; set; }
        public string IsSplitTable { get; set; }
    }
}
