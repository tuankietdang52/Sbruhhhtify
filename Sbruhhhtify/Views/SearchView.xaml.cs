using Microsoft.UI.Xaml.Controls;
using Sbruhhhtify.ViewModels;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sbruhhhtify.Views
{
    public sealed partial class SearchView : UserControl
    {
        public SearchViewModel SearchVM { get; set;}
        public SearchView()
        {
            this.InitializeComponent();
            Load();
        }

        private void Load()
        {
            SearchVM = new SearchViewModel();
            this.DataContext = SearchVM;
        }
    }
}
