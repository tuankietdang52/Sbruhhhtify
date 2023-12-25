using Microsoft.Data.Sqlite;
using Sbruhhhtify.Songs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Sbruhhhtify.Data
{
    public class SongsData
    {
        private static readonly SqliteConnection connection = new($"Data Source={AppDomain.CurrentDomain.BaseDirectory}\\Songs.db; Mode=ReadWriteCreate;");
        public SongsData()
        {
            connection.Open();

            var create = connection.CreateCommand();

            create.CommandText = @"
                CREATE TABLE IF NOT EXISTS SONGDATA (
                    NAME TEXT,
                    PATH TEXT PRIMARY KEY,
                    TIMECREATE DATETIME
                )";

            create.ExecuteNonQuery();
        }

        public void Insert(Song song)
        {
            string name = song.Name;
            string path = song.Songpath;

            using (var transaction = connection.BeginTransaction())
            {
                var insert = new SqliteCommand(@"INSERT INTO SONGDATA VALUES (@name, @path, @datetime)", connection, transaction);
                insert.Parameters.AddWithValue("@name", name);
                insert.Parameters.AddWithValue ("@path", path);
                insert.Parameters.AddWithValue("@datetime", song.CreationTime);

                insert.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public void Delete(string path)
        {
            using (var transaction = connection.BeginTransaction())
            {
                var delete = new SqliteCommand(@"DELETE FROM SONGDATA WHERE PATH = (@path)", connection, transaction);
                delete.Parameters.AddWithValue("@path", path);

                delete.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public ObservableCollection<Song> GetData()
        {
            var list = new ObservableCollection<Song>();

            var get = connection.CreateCommand();
            get.CommandText = (@"
                SELECT * FROM SONGDATA            
            ");

            var data = new object[3];

            using (var reader = get.ExecuteReader())
            {
                while (reader.Read())
                {
                    reader.GetValues(data);
                    
                    var name = Convert.ToString(data[0]);
                    var path = Convert.ToString(data[1]);
                    var date = Convert.ToDateTime(data[2]);

                    list.Add(new Song(name, path, date));
                }
            }

            return list;
        }
    }
}
