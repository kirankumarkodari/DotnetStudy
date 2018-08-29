using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.OleDb;
namespace AcessDbApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string SQLtocheckTableExists = "SELECT COUNT(*) as Exists from MsysObjectsWHERE type = 1 AND name =" + Constants.Tablename;
            string connString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\AccessDB\Pthmu.accdb";
            try
            {
                OleDbConnection DbConn = new OleDbConnection(connString);
                DbConn.Open();
            }
            catch(Exception ex)
            {
                Console.WriteLine("Failed to connect to Database: " + ex.Message);
                
            }

            
            
        }
    }
}
