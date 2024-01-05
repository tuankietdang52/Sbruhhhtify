using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

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
