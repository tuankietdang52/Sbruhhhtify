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

        public SongView(Song song, bool IsChangeSong)
        {
            this.InitializeComponent();
            Load(song, IsChangeSong);
        }

        public void Load(Song song, bool IsChangeSong)
        {
            SongVM = new SongViewModel(song, IsChangeSong);

            this.DataContext = SongVM;

            SongsHandle.AddHistory(song);
        }
    }
}
