using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sbruhhhtify.Data;
using Sbruhhhtify.Interface;
using Sbruhhhtify.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Sbruhhhtify.ViewModels
{
    public partial class SearchViewModel : AppViewModels, IListSong
    {
        public ObservableCollection<Song> SearchList { get; set; }
        private List<Song> songList;

        public ICommand OpenSong { get; set; }

        public SearchViewModel()
        {
            SongsHandle.Subscribe(this);
            Load();
        }

        public override void GenerateCommand()
        {
            OpenSong = new RelayCommand<Song>(ToSongView);
        }

        private void Load()
        {
            SearchList = new ObservableCollection<Song>();
            Update();
        }

        public void Update()
        {
            songList = SongsHandle.SongList?.ToList<Song>() ?? SongsHandle.GetData().ToList<Song>();
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

        protected override void ToSongView(Song song)
        {
            base.ToSongView(song);
            SongsHandle.Unsubcribe(this);
        }
    }
}
