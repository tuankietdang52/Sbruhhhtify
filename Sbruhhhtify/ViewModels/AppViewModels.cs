using CommunityToolkit.Mvvm.ComponentModel;
using Sbruhhhtify.Data;
using Sbruhhhtify.Dialog;
using Sbruhhhtify.Models;
using Sbruhhhtify.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sbruhhhtify.ViewModels
{
    public abstract class AppViewModels : ObservableObject
    {
        protected bool IsChangeSong = true;

        public AppViewModels()
        {
            GenerateCommand();
        }

        public abstract void GenerateCommand();

        protected virtual void ToSongView(Song song)
        {
            if (!song.IsLoaded)
            {
                PopupDialog.ShowError($"Cannot load {song.Name}, please delete and add again");
                return;
            }

            MainViewModel.Instance.View = new SongView(song, IsChangeSong);
        }
    }
}
