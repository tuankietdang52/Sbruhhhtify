using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media.Imaging;
using Sbruhhhtify.Data;
using Sbruhhhtify.Dialog;
using Sbruhhhtify.Error;
using System;
using System.Collections.ObjectModel;
using Windows.Media.Core;
using Windows.Media.Playback;

namespace Sbruhhhtify.Models
{
    public partial class SongModel : ObservableObject
    {
        private ObservableCollection<Song> listSongs;

        [ObservableProperty]
        private Song current;

        [ObservableProperty]
        private Song prev;

        [ObservableProperty]
        private Song next;

        public double LengthSec { get; set; }

        public bool IsGetError = false;

        [ObservableProperty]
        public BitmapImage replayIcon;

        public SongModel(Song current, ObservableCollection<Song> listSongs)
        {
            this.listSongs = listSongs;
            LoadSong(current);
        }

        private void LoadSong(Song current)
        {
            try
            {
                LoadCurrent(current);
                LoadPrevAndNext(current);
                SetReplayIcon(false);
            }
            catch (NotFoundSongException ex)
            {
                IsGetError = true;
                PopupDialog.ShowError("Cannot found song\nError: " + ex);
            }
        }

        private void LoadCurrent(Song current)
        {
            Current = current;

            if (!Current.IsLoaded) throw new NotFoundSongException();

            LengthSec = Current.Length.TotalSeconds;
        }

        private void LoadPrevAndNext(Song current)
        {
            int preindex = -1, nextindex = -1;

            for (int i = 0; i < listSongs.Count; i++)
            {
                if (!current.Songpath.Equals(listSongs[i].Songpath)) continue;

                preindex = i - 1 >= 0 ? i - 1 : listSongs.Count - 1;
                nextindex = i + 1 < listSongs.Count ? i + 1 : 0;
                break;
            }

            if (preindex == -1 || nextindex == -1) throw new NotFoundSongException();

            Prev = listSongs[preindex];
            Next = listSongs[nextindex];
        }

        public void SetReplayIcon(bool IsReplay)
        {
            string path = SongsHandle.IconPath;
            path += IsReplay ? "replay.png" : "notreplay.png";
            ReplayIcon = new BitmapImage(new Uri(path));
        }

        public MediaPlayer GetMedia()
        {
            MediaPlayer media = new MediaPlayer();
            media.Source = MediaSource.CreateFromUri(new Uri(Current.Songpath));
            media.Volume = 1;

            return media;
        }
    }
}
