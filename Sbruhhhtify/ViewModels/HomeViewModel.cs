using CommunityToolkit.Mvvm.Input;
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
    public class HomeViewModel : AppViewModels, IListSong
    {
        public ObservableCollection<History> Histories { get; set; }
        public ICommand OpenSong { get; set; }
        public HomeViewModel() : base()
        {
            SongsHandle.Subscribe(this);
            Update();
        }

        public void Update()
        {
            Histories = SongsHandle.GetHistory();
        }

        public override void GenerateCommand()
        {
            OpenSong = new RelayCommand<Song>(ToSongView);
        }

        protected override void ToSongView(Song song)
        {
            base.ToSongView(song);
            SongsHandle.Unsubcribe(this);
        }
    }
}
