using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Sbruhhhtify.Models;
using Sbruhhhtify.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI;
using Windows.Media.Playback;
using Windows.Media.Core;
using Sbruhhhtify.Dialog;
using Sbruhhhtify.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Sbruhhhtify.Views;
using Microsoft.UI.Xaml.Media.Imaging;

namespace Sbruhhhtify.ViewModels
{
    public partial class SongViewModel : ObservableObject
    {
        [ObservableProperty]
        private SongModel song;

        public SolidColorBrush BgColor { get; set; }

        [ObservableProperty]
        private BitmapImage playStopIcon;

        private bool isPause;
        public bool IsPause { 
            get {  return isPause; }
            set 
            {
                isPause = value;
                ChangeStopPlayIcon();
            } 
        }

        [ObservableProperty]
        private ICommand playStop;

        public ICommand Previous { get; set; }

        public ICommand Next { get; set; }

        public SongViewModel() { }

        public SongViewModel(Song current)
        {
            RandomBgColor();
            LoadModel(current);
            GenerateCommand();
            PlaySong();
        }

        private void RandomBgColor()
        {
            Random rand = new Random();
            Byte[] rgbbyte = new byte[3];

            while (true)
            {
                rand.NextBytes(rgbbyte);
                if (rgbbyte[0] != 255 && rgbbyte[1] != 255 && rgbbyte[2] != 255) break;
            }

            Windows.UI.Color color = Color.FromArgb(30, rgbbyte[0], rgbbyte[1], rgbbyte[2]);

            BgColor = new SolidColorBrush(color);
        }

        private void LoadModel(Song current)
        {
            Song = new SongModel(current);
        }

        private void PlaySong()
        {
            SongModel.Player.Play();
            IsPause = false;
        }

        private void GenerateCommand()
        {
            PlayStop = new RelayCommand(Stop);
            Previous = new RelayCommand(ToPrevSong);
            Next = new RelayCommand(ToNextSong);
        }

        private void Stop()
        {
            SongModel.Player.Pause();
            IsPause = true;
            PlayStop = new RelayCommand(Resume);
        }

        private void Resume()
        {
            IsPause = false;
            SongModel.Player.Play();
            PlayStop = new RelayCommand(Stop);
        }

        private void ToPrevSong()
        {
            MainViewModel.Instance.View = new SongView(Song.Prev);
        }

        private void ToNextSong()
        {
            MainViewModel.Instance.View = new SongView(Song.Next);
        }

        //private void dosomething()
        //{
        //    PopupDialog.Show(PlayStopIcon.UriSource.AbsolutePath);
        //    IsPause = true;
        //    PlayStop = new RelayCommand(Resume);

        //    SongModel.Player.Pause();
        //    PlayStopIcon = new BitmapImage(new Uri($"{SongsHandle.IconPath}replay.png"));
        //    SongModel.Player.Position = new TimeSpan(0, 0, 0);
        //}

        private void ChangeStopPlayIcon()
        {
            Uri source = null;
            if (IsPause)
            {
                source = new Uri($"{SongsHandle.IconPath}play.png");
            }
            else
            {
                source = new Uri($"{SongsHandle.IconPath}pause.png");
            }


            PlayStopIcon = new BitmapImage(source);
        }
    }
}
