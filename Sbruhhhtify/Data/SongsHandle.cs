using Microsoft.Data.Sqlite;
using Sbruhhhtify.Dialog;
using Sbruhhhtify.Interface;
using Sbruhhhtify.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Storage;

namespace Sbruhhhtify.Data
{
    public class SongsHandle
    {
        public static readonly string IconPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\Assets\\Icon\\";

        private static List<IListSong> ObserverList = new List<IListSong>();

        public static ObservableCollection<Song> SongList { get; private set; } 
        public static ObservableCollection<Song> RandomList { get; private set; }

        private static bool isRandom = false;
        public static bool IsRandom
        {
            get => isRandom;
            set
            {
                isRandom = value;
                if (isRandom) InitRandomList();
                Notify();
            }
        }

        public SongsHandle() { }

        public static void Notify()
        {
            SongList = GetData();
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
            SongList = data.GetData();
            return SongList;
        }

        private static void InitRandomList()
        {
            var templist = GetData();
            var randomlist = new ObservableCollection<Song>();

            int i = 0;
            int length = templist.Count;

            while (i < length)
            {
                Random rand = new Random();
                int index = rand.Next(0, templist.Count);
                
                var item = templist[index];

                randomlist.Add(item);
                templist.RemoveAt(index);

                i++;
            }

            RandomList = randomlist;
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