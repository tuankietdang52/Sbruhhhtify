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
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Sbruhhhtify.Interface;
using System.Collections.ObjectModel;

namespace Sbruhhhtify.ViewModels
{
    public partial class SongViewModel : AppViewModels, IListSong
    {
        private readonly Microsoft.UI.Dispatching.DispatcherQueue mainDispatcher = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();

        public static SongViewModel Instance { get; private set; }

        public ObservableCollection<Song> listSong { get; private set; }

        [ObservableProperty]
        private SongModel song;

        public SolidColorBrush BgColor { get; set; }

        [ObservableProperty]
        private BitmapImage playStopIcon;

        [ObservableProperty]
        private double position;

        public static MediaPlayer SongPlayer;

        private bool isReplay = false;
        public bool IsReplay
        {
            get => isReplay;
            set
            {
                isReplay = value;
                SetEndEvent();
            }
        }

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

        // test :vv
        public ICommand Previous => new RelayCommand(ToPrevSong);

        public ICommand Next => new RelayCommand(ToNextSong);
        public ICommand Replay => new RelayCommand(SetReplayMode);

        public SongViewModel() { }

        public SongViewModel(Song current) : base()
        {
            SongsHandle.Subscribe(this);
            Update();

            RandomBgColor();
            LoadModel(current);

            Instance = this;

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
            Song = new SongModel(current, listSong);

            if (Song.IsGetError) return;

            InitSongPlayer();
            InitTime();
        }

        private void InitSongPlayer()
        {
            if (SongPlayer is not null) SongPlayer.Dispose();

            SongPlayer = new MediaPlayer();
            SongPlayer = Song.GetMedia();
            SetEndEvent();
        }

        private void SetEndEvent()
        {
            SongPlayer.MediaEnded -= ReplaySong;
            SongPlayer.MediaEnded -= HandleNextSong;

            SongPlayer.MediaEnded += IsReplay ? ReplaySong : HandleNextSong;
        }

        private void InitTime()
        {
            time = new System.Timers.Timer(1000);
            time.AutoReset = true;

            // try to remove last event
            try { time.Elapsed -= ElapsedSecond; }
            catch { }

            time.Elapsed += ElapsedSecond;
        }

        private void ElapsedSecond(Object source, System.Timers.ElapsedEventArgs e)
        {
            if (Position >= Song.LengthSec) return;
            mainDispatcher.TryEnqueue(() => Position++);
        }

        public override void GenerateCommand() => PlayStop = new RelayCommand(Stop);

        public void Update()
        {
            var list = () =>
            {
                if (SongsHandle.IsRandom) return SongsHandle.RandomList;
                else return SongsHandle.SongList ?? SongsHandle.GetData();
            };

            listSong = list();
        }

        private void SetReplayMode()
        {
            IsReplay = !IsReplay;

            Song.SetReplayIcon(IsReplay);
        }

        private void PlaySong()
        {
            Position = SongPlayer.Position.TotalSeconds;

            Resume();
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
            SongPlayer.Play();

            IsPause = false;

            PlayStop = new RelayCommand(Stop);
        }

        private void ToPrevSong()
        {
            base.ToSongView(Song.Prev);
            SongsHandle.Unsubcribe(this);
        }

        private void ToNextSong()
        {
            base.ToSongView(Song.Next);
            SongsHandle.Unsubcribe(this);
        }

        private void ReplaySong(MediaPlayer sender, object arg)
        {
            SongPlayer.Pause();
            SongPlayer.Position = new TimeSpan(0, 0, 0);
            time.Stop();

            try
            {
                // Binding property can only be changed when in main thread
                // In an event where not in main thread so the app will throw
                // System.Runtime.InteropServices.COMException (0x8001010E)
                mainDispatcher.TryEnqueue(PlaySong);
            }
            catch (Exception ex)
            {
                PopupDialog.ShowError($"{ex}");
            }
        }

        private void HandleNextSong(MediaPlayer sender, object arg)
        {
            mainDispatcher.TryEnqueue(ToNextSong);
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

            TimeSpan valueSec = TimeSpan.FromSeconds(value);
            SongPlayer.Position = valueSec;

            mainDispatcher.TryEnqueue(() =>
            {
                Position = value;
            });
        }
    }
}