using MostraRota.Droid.Implementations;
using MostraRota.Interfaces;
using SQLite;
using System.IO;

[assembly: Xamarin.Forms.Dependency(typeof(SQLiteConnection_Droid))]
namespace MostraRota.Droid.Implementations
{
    public class SQLiteConnection_Droid : ISQLiteConnection
    {
        public SQLiteConnection DBConnection()
        {
            var path = Path.Combine(System.Environment.
                                    GetFolderPath(System.Environment.
                                    SpecialFolder.Personal), "mostrarota.db3");
            return new SQLiteConnection(path);
        }
    }
}