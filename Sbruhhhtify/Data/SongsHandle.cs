using Microsoft.Data.Sqlite;
using Microsoft.UI.Xaml.Controls;
using Sbruhhhtify.Dialog;
using Sbruhhhtify.Interface;
using Sbruhhhtify.Models;
using Sbruhhhtify.Error;
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
        public static readonly string IconPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\Assets\\Icon\\";
        
        private static ObservableCollection<Song> list = GetData();

        private static List<IListSong> ObserverList = new List<IListSong>();

        private static bool isRandom = false;
        public static bool IsRandom
        {
            get => isRandom;
            set
            {
                isRandom = value;
                list = isRandom ? GetRandom() : GetData();
            }
        }

        public SongsHandle() { }

        public static void Notify()
        {
            foreach (var list in ObserverList)
            {
                list.Update();
            }
        }

        public static void Subscribe(IListSong subscriber)
        {
            ObserverList.Add(subscriber);
        }

        public static void Unsubcribe(IListSong subscriber)
        {
            ObserverList.Remove(subscriber);
        }

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

        private static ObservableCollection<Song> GetRandom()
        {
            var templist = GetData();
            var randomlist = new ObservableCollection<Song>();

            int i = 0;
            while (i < templist.Count)
            {
                Random rand = new Random();
                int index = rand.Next(0, templist.Count);
                
                var item = templist[index];

                randomlist.Add(item);
                templist.RemoveAt(index);

                i++;
            }

            return randomlist;
        }

        public static void Insert(Song song)
        {
            try
            {
                SongsData data = new SongsData();
                data.Insert(song);

                Notify();
            } 
            catch (SqliteException ex)
            {
                PopupDialog.ShowError($"Song is already added {ex.Message}");
            }
        }

        public static void Delete(string path)
        {
            try
            {
                SongsData data = new SongsData();
                data.Delete(path);

                Notify();
            }
            catch (Exception ex)
            {
                PopupDialog.ShowError(ex.Message);
            }
        }

        public static Song GetPreviousSong(Song current)
        {
            for (int index = 0; index < list.Count; index++)
            {
                if (!current.Songpath.Equals(list[index].Songpath)) continue;

                int previous = index - 1 < 0 ? list.Count - 1 : index - 1;

                return list[previous];
            }

            throw new NotFoundSongException();
        }

        public static Song GetNextSong(Song current)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (!current.Songpath.Equals(list[i].Songpath)) continue;

                int next = i + 1 >= list.Count ? 0 : i + 1;

                return list[next];
            }

            throw new NotFoundSongException();
        }

        public static ObservableCollection<History> GetHistory()
        {
            SongsData data = new SongsData();

            return data.GetHistory();
        }

        public static void AddHistory(Song song)
        {
            string path = song.Songpath;
            History history = new History(song, DateTime.Now);

            SongsData data = new SongsData();

            var list = data.GetHistory();
            data.ClearHistory();

            data.AddHistory(history);

            foreach (var item in list)
            {
                if (item.Song.Songpath == path) continue;

                data.AddHistory(item);
            }

            Notify();
        }
    }
}