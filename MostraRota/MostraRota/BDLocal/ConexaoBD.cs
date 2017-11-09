using MostraRota.Interfaces;
using SQLite;
using System;
using Xamarin.Forms;

namespace MostraRota.BDLocal
{
    public class ConexaoBD
    {
        public SQLiteConnection DBConnection { get; set; }

        public ConexaoBD()
        {
            // conecta ao banco de dados local (ou cria, caso ele ainda não exista)
            DBConnection = DependencyService.Get<ISQLiteConnection>().DBConnection();

            // cria ou altera as tabelas, se for necessário
            DBConnection.CreateTable<UsuariosBD>();
            DBConnection.CreateTable<RotasBD>();
            DBConnection.CreateTable<CoordenadasBD>();
        }

        public int GetLastRowId(string tabela, string chave)
        {
            SQLiteCommand cmd = DBConnection.CreateCommand("SELECT last_insert_rowid()");
            int i = cmd.ExecuteScalar<int>();

            cmd.CommandText = "SELECT " + chave + " FROM " + tabela + " WHERE rowid = " + i.ToString();
            i = cmd.ExecuteScalar<int>();

            return i;
        }
    }
}
