using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Sbruhhhtify.Models;
using Sbruhhhtify.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sbruhhhtify.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        public ObservableCollection<MainModel> MainModels { get; set; }

        public static MainViewModel Instance { get; set; }

        [ObservableProperty]
        private UserControl view;

        public ICommand ChangeContent { get; set; }

        public MainViewModel()
        {
            LoadModels();
            ChangeContent = new RelayCommand<string>(HandleChangeContent);
            View = new HomeView();
            Instance = this;
        }


        public void LoadModels()
        {
            ObservableCollection<MainModel> list = new ObservableCollection<MainModel>();

            list.Add(new MainModel("homebutton", "sbruhhhtify.png", true));
            list.Add(new MainModel("searchbutton", "search.png", false));
            list.Add(new MainModel("folderbutton", "folder.png", false));

            MainModels = list;
        }

        public void HandleChangeContent(string name)
        {
            ChangeBgButton(name);

            switch (name)
            {
                case "homebutton":
                    View = new HomeView();
                    break;

                case "searchbutton":
                    View = null;
                    break;

                case "folderbutton":
                    View = new FolderView();
                    break;
            }
        }

        private void ChangeBgButton(string name)
        {
            foreach (var model in MainModels)
            {
                if (model.Name != name) model.IsActive = false;
                else model.IsActive = true;
            }
        }
    }
}
