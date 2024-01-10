﻿using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sbruhhhtify.Data;
using Sbruhhhtify.Dialog;
using Sbruhhhtify.Interface;
using Sbruhhhtify.Models;
using Sbruhhhtify.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sbruhhhtify.ViewModels
{
    public partial class SearchViewModel : IListSong
    {
        public ObservableCollection<Song> SearchList { get; set; }
        private List<Song> songList; 

        public SearchViewModel()
        {
            SongsHandle.Subscribe(this);
            Load();
        }

        private void Load()
        {
            SearchList = new ObservableCollection<Song>();
            Update();
        }

        public void Update()
        {
            songList = SongsHandle.GetData().ToList<Song>();
        }

        public void Type(object sender, RoutedEventArgs arg)
        {
            SearchList.Clear();
            var searchbar = sender as TextBox;
            var value = searchbar.Text.ToLower();

            if (value == string.Empty) return;

            foreach (var song in songList)
            {
                var name = song.Name.ToLower();
                if (!name.Contains(value)) continue;

                SearchList.Add(song);
            }
        }

        [RelayCommand]
        public void ToSongView(Song song)
        {
            if (!song.IsLoaded)
            {
                PopupDialog.ShowError($"Cannot load {song.Name}, please delete and add again");
                return;
            }

            SongsHandle.Unsubcribe(this);
            MainViewModel.Instance.View = new SongView(song);
        }
    }
}
