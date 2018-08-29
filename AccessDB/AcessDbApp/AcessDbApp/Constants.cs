using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AcessDbApp
{
    public static class Constants
    {
        public const string DbPath= @"C:\AccessDB\Pthmu.accdb";
        public const string Tablename = "MyTable";
        public const string Coloumn0Name = "RowID";
        public const string Coloumn0Datatype = "AutoNumber";
        public const string Coloumn1Name = "Name";
        public const string Coloumn1Datatype = "TEXT(100)";
        public const string Coloumn2Name = "Number";
        public const string Coloumn2Datatype = "INTEGER";
        public const string Coloumn3Name = "Cost";
        public const string Coloumn3Datatype = "Currency";
        public const string Coloumn4Name = "CreatedTime";
        public const string Coloumn4Datatype = "Date/Time";
        
    }
}
