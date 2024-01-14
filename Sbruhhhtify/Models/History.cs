using CommunityToolkit.Mvvm.ComponentModel;
using System;

namespace Sbruhhhtify.Models
{
    public partial class History : ObservableObject
    {
        [ObservableProperty]
        private Song song;

        [ObservableProperty]
        private DateTime timeopen;

        public string Time { get; }

        public History() { }
        
        public History(Song song, DateTime timeopen)
        {
            this.Song = song;
            this.timeopen = timeopen;
            Time = this.timeopen.ToString("MM/dd/yyyy");
        }
    }
}
