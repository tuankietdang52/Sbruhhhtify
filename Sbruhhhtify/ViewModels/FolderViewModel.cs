using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Sbruhhhtify.Data;
using Sbruhhhtify.Models;
using Sbruhhhtify.Interface;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Windows.Storage.Pickers;

namespace Sbruhhhtify.ViewModels
{
    public partial class FolderViewModel : AppViewModels, IListSong
    {
        [ObservableProperty]
        private ObservableCollection<Song> listSong;
        public ICommand Add {  get; set; }
        public ICommand Delete { get; set; }
        public ICommand OpenSong { get; set; }
        public ICommand OpenRandom { get; set; }

        public FolderViewModel() : base()
        {
            SongsHandle.Subscribe(this);
            Update();
        }

        public void Update()
        {
            ListSong = SongsHandle.SongList ?? SongsHandle.GetData();
        }

        public override void GenerateCommand()
        {
            Add = new RelayCommand(HandleAddSong);
            Delete = new RelayCommand<string>(HandleDeleteSong);
            OpenSong = new RelayCommand<Song>(ToSongView);
            OpenRandom = new RelayCommand(ToRandomSong);
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

        protected override void ToSongView(Song song)
        {
            SongsHandle.IsRandom = false;
            base.ToSongView(song);
            SongsHandle.Unsubcribe(this);
        }

        public void ToRandomSong()
        {
            SongsHandle.IsRandom = true;
            var list = SongsHandle.RandomList;

            base.ToSongView(list[0]);
            SongsHandle.Unsubcribe(this);
        }
    }
}
