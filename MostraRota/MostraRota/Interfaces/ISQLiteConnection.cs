using SQLite;

namespace MostraRota.Interfaces
{
    public interface ISQLiteConnection
    {
        SQLiteConnection DBConnection();
    }
}
