﻿using CommunityToolkit.Mvvm.ComponentModel;
using Sbruhhhtify.Data;
using Sbruhhhtify.Dialog;
using Sbruhhhtify.Error;
using System;
using System.Threading.Tasks;
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

        public bool IsLoaded = true;

        public Song Instance { get; set; }

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

        public Song()
        {
            Instance = this;
        }

        public Song(string Name, string Songpath, DateTime CreationTime)
        {
            this.Name = Name;
            this.Songpath = Songpath;
            this.CreationTime = CreationTime;
            Instance = this;
            HandleSetDuration();
        }

        public void SetDate()
        {
            // date type string
            Date = CreationTime.ToString("MM/dd/yyyy");
        }

        public void HandleSetDuration()
        {
            try
            {
                StorageFile file = null;
                MusicProperties properties = null;

                Task task = Task.Run(async () =>
                {
                    file = await StorageFile.GetFileFromPathAsync(Songpath);
                    properties = await file.Properties.GetMusicPropertiesAsync();
                });

                task.Wait();
                
                if (file is null) throw new NotFoundSongException();

                Length = properties.Duration;
            }
            catch (Exception ex) {
                IsLoaded = false;

                SongsHandle.Delete(Songpath);
                PopupDialog.ShowError($"Cannot load {Name}\nError:" + ex);
            }
        }

        private void SetDuration()
        {
            Duration = Length.ToString(@"hh\:mm\:ss");
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
