using Microsoft.Data.Sqlite;
using Sbruhhhtify.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Formats.Asn1.AsnWriter;

namespace Sbruhhhtify.Data
{
    public class SongsData
    {
        private static readonly SqliteConnection connection = new($"Data Source={AppDomain.CurrentDomain.BaseDirectory}\\Songs.db; Mode=ReadWriteCreate;");
        public SongsData()
        {
            connection.Open();

            CreateTableSong();
            CreateTableHistory();
        }

        private void CreateTableSong()
        {
            var create = connection.CreateCommand();

            create.CommandText = @"
                CREATE TABLE IF NOT EXISTS SONGDATA (
                    NAME TEXT,
                    PATH TEXT PRIMARY KEY,
                    TIMECREATE DATETIME
                )";

            create.ExecuteNonQuery();
        }

        private void CreateTableHistory()
        {
            var create = connection.CreateCommand();

            create.CommandText = @"
                CREATE TABLE IF NOT EXISTS HISTORY (
                    PATH TEXT NOT NULL UNIQUE,
                    TIMEOPEN DATETIME,
                    FOREIGN KEY (PATH) REFERENCES SONGDATA(PATH) ON DELETE CASCADE
            )";

            create.ExecuteNonQuery();
        }

        public void Insert(Song song)
        {
            string name = song.Name;
            string path = song.Songpath;
            var date = song.CreationTime;

            using (var transaction = connection.BeginTransaction())
            {
                var insert = new SqliteCommand(@"INSERT INTO SONGDATA VALUES (@name, @path, @timecreate)", connection, transaction);
                insert.Parameters.AddWithValue("@name", name);
                insert.Parameters.AddWithValue ("@path", path);
                insert.Parameters.AddWithValue("@timecreate", date);

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
                SELECT NAME, PATH, TIMECREATE FROM SONGDATA     
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

        public void ClearHistory()
        {
            using (var transaction = connection.BeginTransaction())
            {
                var delete = new SqliteCommand(@"DELETE FROM HISTORY", connection, transaction);

                delete.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public void AddHistory(History history)
        {
            string path = history.Song.Songpath;
            var date = history.Timeopen;

            using (var transaction = connection.BeginTransaction())
            {
                var insert = new SqliteCommand(@"INSERT INTO HISTORY VALUES (@path, @timeopen)", connection, transaction);
                insert.Parameters.AddWithValue("@path", path);
                insert.Parameters.AddWithValue("@timeopen", date);

                insert.ExecuteNonQuery();
                transaction.Commit();
            }
        }

        public ObservableCollection<History> GetHistory()
        {
            var list = new ObservableCollection<History>();

            var get = connection.CreateCommand();
            get.CommandText = (@"
                SELECT NAME, HISTORY.PATH, TIMECREATE, TIMEOPEN
                FROM HISTORY, SONGDATA
                WHERE HISTORY.PATH = SONGDATA.PATH
            ");

            var data = new object[4];

            using (var reader = get.ExecuteReader())
            {
                while (reader.Read())
                {
                    reader.GetValues(data);

                    var name = Convert.ToString(data[0]);
                    var path = Convert.ToString(data[1]);
                    var date = Convert.ToDateTime(data[2]);
                    var history = Convert.ToDateTime(data[3]);

                    Song song = new Song(name, path, date);
                    list.Add(new History(song, history));
                }
            }

            return list;
        }
    }
}
