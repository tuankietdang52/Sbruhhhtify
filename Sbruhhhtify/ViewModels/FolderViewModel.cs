using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Sbruhhhtify.Data;
using Sbruhhhtify.Models;
using Sbruhhhtify.Interface;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;
using Sbruhhhtify.Views;
using Sbruhhhtify.Dialog;
using System.Xml.Linq;

namespace Sbruhhhtify.ViewModels
{
    public partial class FolderViewModel : ObservableObject, IListSong
    {
        [ObservableProperty]
        private ObservableCollection<Song> listSong;
        public ICommand Add {  get; set; }
        public ICommand Delete { get; set; }
        public ICommand OpenSong { get; set; }

        public FolderViewModel()
        {
            SongsHandle.Subscribe(this);
            Update();
            GenerateCommand();
        }

        public void Update()
        {
            ListSong = SongsHandle.GetData();
        }

        private void GenerateCommand()
        {
            Add = new RelayCommand(HandleAddSong);
            Delete = new RelayCommand<string>(HandleDeleteSong);
            OpenSong = new RelayCommand<Song>(ToSongView);
        }

        private async void HandleAddSong()
        {
            // https://github.com/microsoft/WindowsAppSDK/issues/1188

            FileOpenPicker filepicker = new FileOpenPicker();

            var windowhandle = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.MainView);

            WinRT.Interop.InitializeWithWindow.Initialize(filepicker, windowhandle);

            filepicker.FileTypeFilter.Add(".mp3");
            var file = await filepicker.PickSingleFileAsync();

            var song = SongsHandle.ConvertFileToSong(file);

            SongsHandle.Insert(song);
        }

        private void HandleDeleteSong(string path)
        {
            SongsHandle.Delete(path);
        }

        private void ToSongView(Song song)
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
