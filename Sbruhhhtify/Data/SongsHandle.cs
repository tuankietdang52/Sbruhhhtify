using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using Sbruhhhtify.Error;
using Sbruhhhtify.Interface;
using Sbruhhhtify.Songs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.UI.Popups;

namespace Sbruhhhtify.Data
{
    public class SongsHandle
    {
        public SongsHandle() { }

        public static Song ConvertFileToSong(StorageFile file)
        {
            Song song = new Song(file.DisplayName, file.Path, file.DateCreated.Date);

            return song;
        }

        public static ObservableCollection<Song> GetData()
        {
            SongsData data = new SongsData();
            return data.GetData();
        }

        public static void Insert(Song song, IListSong<Song> ViewModel)
        {
            try
            {
                SongsData data = new SongsData();
                data.Insert(song);

                var list = data.GetData();

                ViewModel.SetList(list);
            } 
            catch (SqliteException ex)
            {
                SongException.IsAlreadyAdded(ex.Message);
            }
        }

        public static void Delete(string path, IListSong<Song> ViewModel)
        {
            try
            {
                SongsData data = new SongsData();
                data.Delete(path);

                var list = data.GetData();

                ViewModel.SetList(list);
            }
            catch (Exception ex)
            {
                SongException.UnknowException(ex.Message);
            }
        }
    }
}