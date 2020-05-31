using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cygnus2_0.BaseDatos.sqlite
{
    public class DbContext
    {
        private const string DBName = "Cygnus.db";

        public static void test()
        {
            using (var ctx = GetInstance())
            {
                var query = "select * from paths";

                using (var command = new SQLiteCommand(query, ctx))
                {
                    SQLiteDataReader reader = command.ExecuteReader();
                }
            }
        }

        public static SQLiteConnection GetInstance()
        {
            var db = new SQLiteConnection(
                string.Format("Data Source={0};Version=3;", DBName)
            );
            db.Open();

            return db;
        }
    }
}
