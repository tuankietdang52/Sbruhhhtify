using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Sbruhhhtify.Data;
using Sbruhhhtify.Interface;
using Sbruhhhtify.Songs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage.FileProperties;
using Windows.Storage.Pickers;

namespace Sbruhhhtify.ViewModels
{
    public partial class FolderViewModel : ObservableObject, IListSong<Song>
    {
        [ObservableProperty]
        private ObservableCollection<Song> listSong;
        public ICommand Add {  get; set; }
        public ICommand Delete { get; set; }

        public FolderViewModel()
        {
            GetData();
            Add = new RelayCommand(HandleAddSong);
            Delete = new RelayCommand<string>(HandleDeleteSong);
        }

        public void GetData()
        {
            ListSong = SongsHandle.GetData();
        }

        public void SetList(ObservableCollection<Song> list)
        {
            ListSong = list;
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

            SongsHandle.Insert(song, this);
        }

        public void HandleDeleteSong(string path)
        {
            SongsHandle.Delete(path, this);
        }
    }
}
