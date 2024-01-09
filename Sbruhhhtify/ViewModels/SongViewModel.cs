using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using Sbruhhhtify.Models;
using System;
using Windows.UI;
using Windows.Media.Playback;
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

        public static MediaPlayer SongPlayer;

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

        private readonly Microsoft.UI.Dispatching.DispatcherQueue mainthread = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

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

            if (Song.IsGetError) return;

            InitSongPlayer();
        }

        private void InitSongPlayer()
        {
            if (SongPlayer is not null) SongPlayer.Dispose();
            SongPlayer = new MediaPlayer();
            SongPlayer = Song.GetMedia();
            SongPlayer.MediaEnded += EndSong;
        }

        private void PlaySong()
        {
            SongPlayer.Play();
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
            SongPlayer.Pause();
            IsPause = true;
            PlayStop = new RelayCommand(Resume);
        }

        private void Resume()
        {
            IsPause = false;
            SongPlayer.Play();
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

        private void EndSong(MediaPlayer sender, object arg)
        {
            try
            {
                // Binding property can only be changed when in main thread
                // In an event where not in main thread so the app will throw
                // System.Runtime.InteropServices.COMException (0x8001010E)
                mainthread.TryEnqueue(() =>
                {
                    IsPause = true;
                    PlayStop = new RelayCommand(Resume);

                    SongPlayer.Pause();
                    PlayStopIcon = new BitmapImage(new Uri($"{SongsHandle.IconPath}replay.png"));
                    SongPlayer.Position = new TimeSpan(0, 0, 0);
                });
            }
            catch (Exception ex)
            {
                PopupDialog.ShowError($"{ex}");
            }
        }

        private void ChangeStopPlayIcon()
        {
            Uri source;
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