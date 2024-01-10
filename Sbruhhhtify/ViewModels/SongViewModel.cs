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
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Timers;

namespace Sbruhhhtify.ViewModels
{
    public partial class SongViewModel : ObservableObject
    {
        private readonly Microsoft.UI.Dispatching.DispatcherQueue mainthread = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        [ObservableProperty]
        private SongModel song;

        public SolidColorBrush BgColor { get; set; }

        [ObservableProperty]
        private BitmapImage playStopIcon;

        [ObservableProperty]
        private double position;

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

        private System.Timers.Timer time;

        [ObservableProperty]
        private ICommand playStop;

        public ICommand Previous => new RelayCommand(ToPrevSong);

        public ICommand Next => new RelayCommand(ToNextSong);

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

        private void InitTime()
        {
            Position = 0;

            try { time.Elapsed -= ElapsedSecond; }
            catch { }

            time = new System.Timers.Timer();
            time.Interval = 1000;
            time.Elapsed += ElapsedSecond;
        }

        private void ElapsedSecond(Object source, System.Timers.ElapsedEventArgs e)
        {
            mainthread.TryEnqueue(() =>
            {
                if (Position >= Song.LengthSec) return;
                Position++;
            });
        }

        private void GenerateCommand() => PlayStop = new RelayCommand(Stop);

        private void PlaySong()
        {
            InitTime();

            SongPlayer.Play();
            time.Start();
            IsPause = false;
            PlayStop = new RelayCommand(Stop);
        }

        private void Stop()
        {
            SongPlayer.Pause();
            time.Stop();
            IsPause = true;
            PlayStop = new RelayCommand(Resume);
        }

        private void Resume()
        {
            time.Start();
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
                    PlayStop = new RelayCommand(PlaySong);

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

        public void ChangePosition(object sender, RoutedEventArgs e)
        {
            var slider = sender as Slider;
            var value = slider.Value;

            mainthread.TryEnqueue(() =>
            {
                Position = value;

                TimeSpan time = TimeSpan.FromSeconds(value);

                SongPlayer.Position = time;
            });
        }
    }
}