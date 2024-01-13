using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Sbruhhhtify.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sbruhhhtify.Models
{
    public partial class MainModel : ObservableObject
    {
        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private BitmapImage imgsource;

        [ObservableProperty]
        private SolidColorBrush color;

        [ObservableProperty]
        private Windows.UI.Color hovercolor;

        private string source;
        public string Source {
            get { return source; }
            set
            {
                source = value;
                SetImg();
            }
        }

        private bool isActive;
        public bool IsActive { 
            get { return isActive; } 
            set { 
                isActive = value;
                ChangeColor();
            }
        }

        public MainModel() { }

        public MainModel(string Name, string Source, bool IsActive)
        {
            this.Name = Name;
            this.Source = Source;
            this.IsActive = IsActive;

            SetHoverColor(48, 48, 48);
        }

        private void ChangeColor()
        {
            if (!isActive) SetColor(48, 48, 48);
            else SetColor(73, 72, 72);
        }

        public void SetColor(byte Red, byte Green, byte Blue)
        {
            var color = Windows.UI.Color.FromArgb(255, Red, Green, Blue);

            Color = new SolidColorBrush(color);
        }

        public void SetHoverColor(byte Red, byte Green, byte Blue)
        {
            Hovercolor = Windows.UI.Color.FromArgb(255, Red, Green, Blue);
        }

        private void SetImg()
        {
            Imgsource = new BitmapImage(new Uri(SongsHandle.IconPath + Source));
        }
    }
}
