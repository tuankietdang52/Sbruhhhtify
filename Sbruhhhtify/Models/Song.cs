using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.FileProperties;

namespace Sbruhhhtify.Models
{
    public partial class Song : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string duration;

        [ObservableProperty]
        private string date;

        [ObservableProperty]
        private string songpath;

        private DateTime creationtime;
        public DateTime CreationTime { 
            get { return creationtime; } 
            set
            {
                creationtime = value;
                SetDate();
            }
        }

        private TimeSpan length;
        public TimeSpan Length
        {
            get { return length; }
            set
            {
                length = value;
                SetDuration();
            }
        }

        private MediaPlayer mediaPlayer = new MediaPlayer();

        public Song() { }

        public Song(string Name, string Songpath, DateTime CreationTime)
        {
            this.Name = Name;
            this.Songpath = Songpath;
            this.CreationTime = CreationTime;
            HandleSetDuration();
        }

        public void SetDate()
        {
            // date type string
            Date = CreationTime.ToString("MM/dd/yyyy");
        }

        public async void HandleSetDuration()
        {
            StorageFile file = await StorageFile.GetFileFromPathAsync(Songpath);
            MusicProperties properties = await file.Properties.GetMusicPropertiesAsync();

            Length = properties.Duration;
        }

        private void SetDuration()
        {
            Duration = Length.ToString(@"hh\:mm\:ss");
        }
    }
}
