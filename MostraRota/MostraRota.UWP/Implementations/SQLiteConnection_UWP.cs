using MostraRota.Interfaces;
using MostraRota.UWP.Implementations;
using SQLite;
using System.IO;
using Windows.Storage;

[assembly: Xamarin.Forms.Dependency(typeof(SQLiteConnection_UWP))]
namespace MostraRota.UWP.Implementations
{
    public class SQLiteConnection_UWP : ISQLiteConnection
    {
        public SQLiteConnection DBConnection()
        {
            var path = Path.Combine(ApplicationData.Current.LocalFolder.Path,
                                    "mostrarota.db3");
            return new SQLiteConnection(path);
        }
    }
