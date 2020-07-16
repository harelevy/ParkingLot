using Dapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParkingLotProject.Logic
{
    public class SqliteDataAccess
    {
        public static List<VehicleModel> LoadVehicles()
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                var output = cnn.Query<VehicleModel>("select * from Vehicle", new DynamicParameters());
                return output.ToList();
            }
        }

        public static void SaveVehicle(VehicleModel vehicle)
        {
            using (IDbConnection cnn = new SQLiteConnection(LoadConnectionString()))
            {
                cnn.Execute("insert into Vehicle (Id, Decision,TimeStamp) values (@Id, @Decision,@TimeStamp)", vehicle);
            }
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
