﻿using CommunityToolkit.Mvvm.Input;
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
using System.Windows.Input;
using Windows.Storage.Pickers;

namespace Sbruhhhtify.ViewModels
{
    public class HomeViewModel : IListSong
    {
        public ObservableCollection<History> Histories { get; set; }
        public ICommand OpenSong { get; set; }
        public HomeViewModel()
        {
            SongsHandle.Subscribe(this);
            Update();
            GenerateCommand();
        }

        public void Update()
        {
            Histories = SongsHandle.GetHistory();
        }

        private void GenerateCommand()
        {
            OpenSong = new RelayCommand<Song>(ToSongView);
        }

        private void ToSongView(Song song)
        {
            if (!song.IsLoaded)
            {
                PopupDialog.ShowError($"Cannot load {song.Name}, please delete and add again");
                return;
            }

            MainViewModel.Instance.View = new SongView(song);
            SongsHandle.Unsubcribe(this);
        }
    }
}
