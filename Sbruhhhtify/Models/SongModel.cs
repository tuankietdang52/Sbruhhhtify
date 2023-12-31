using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using Sbruhhhtify.Data;
using Sbruhhhtify.Dialog;
using Sbruhhhtify.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.VoiceCommands;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI;

namespace Sbruhhhtify.Models
{
    public partial class SongModel : ObservableObject
    {
        [ObservableProperty]
        private Song current;

        [ObservableProperty]
        private Song prev;

        [ObservableProperty]
        private Song next;

        public bool IsGetError = false;

        public SongModel(Song Song)
        {
            LoadSong(Song);
        }

        private void LoadSong(Song Song)
        {
            try
            {
                LoadCurrent(Song);
                LoadPrevAndNext(Song);
            }
            catch (NotFoundSongException ex)
            {
                IsGetError = true;
                PopupDialog.ShowError("Cannot found song\nError: " + ex);
            }
        }

        private void LoadCurrent(Song Song)
        {
            Current = Song;

            if (!Current.IsLoaded) throw new NotFoundSongException();
        }

        private void LoadPrevAndNext(Song Song)
        {
            Prev = SongsHandle.GetPreviousSong(Song);
            Next = SongsHandle.GetNextSong(Song);
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
