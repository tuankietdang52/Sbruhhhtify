using Microsoft.UI.Xaml.Controls;
using Sbruhhhtify.Models;
using Sbruhhhtify.ViewModels;
using Sbruhhhtify.Error;
using Sbruhhhtify.Data;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sbruhhhtify.Views
{
    public sealed partial class SongView : UserControl
    {
        public SongViewModel SongVM { get; set; }
        public SongView()
        {
            throw new NotFoundSongException();
        }

        public SongView(Song song)
        {
            this.InitializeComponent();
            Load(song);
        }

        public void Load(Song song)
        {
            SongVM = new SongViewModel(song);

            this.DataContext = SongVM;

            SongsHandle.AddHistory(song);
        }
    }
}
