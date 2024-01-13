using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Sbruhhhtify.Data;
using Sbruhhhtify.Interface;
using Sbruhhhtify.Models;
using Sbruhhhtify.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sbruhhhtify.ViewModels
{
    public partial class MainViewModel : AppViewModels
    {
        public ObservableCollection<MainModel> MainModels { get; set; }

        public static MainViewModel Instance { get; private set; }

        public MainModel CurrentButton { get; set; }

        [ObservableProperty]
        private UserControl view;

        public ICommand ChangeContent { get; set; }
        public ICommand OpenCurrentSong { get; set; }

        public MainViewModel() : base()
        {
            LoadModels();
            View = new HomeView();
            Instance = this;
        }

        public override void GenerateCommand()
        {
            ChangeContent = new RelayCommand<string>(HandleChangeContent);
            OpenCurrentSong = new RelayCommand(ToCurrentSong);
        }

        private void LoadModels()
        {
            ObservableCollection<MainModel> list = new ObservableCollection<MainModel>();

            list.Add(new MainModel("homebutton", "sbruhhhtify.png", true));
            list.Add(new MainModel("searchbutton", "search.png", false));
            list.Add(new MainModel("folderbutton", "folder.png", false));

            CurrentButton = new MainModel("currentbutton", "current.png", false);

            MainModels = list;
        }

        private void HandleChangeContent(string name)
        {
            try
            {
                SongsHandle.Unsubcribe((IListSong)View.DataContext);
            }
            catch { }

            ChangeBgButton(name);

            switch (name)
            {
                case "homebutton":
                    View = new HomeView();
                    break;

                case "searchbutton":
                    View = new SearchView();
                    break;

                case "folderbutton":
                    View = new FolderView();
                    break;
            }
        }

        private void ChangeBgButton(string name)
        {
            foreach (var model in MainModels)
            {
                if (model.Name != name) model.IsActive = false;
                else model.IsActive = true;
            }
        }

        private void ToCurrentSong()
        {
            var current = SongViewModel.Instance.Song.Current;

            if (View is SongView || current is null) return;

            IsChangeSong = false;
            base.ToSongView(current);
        }
    }
}