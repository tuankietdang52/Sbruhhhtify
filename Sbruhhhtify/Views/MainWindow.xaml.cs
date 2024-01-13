using ABI.Windows.UI;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Sbruhhhtify.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Services.Maps;
using Windows.UI.Popups;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Sbruhhhtify
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private static Window mainView;
        public static Window MainView { get { return mainView; } }

        public MainViewModel mainVM { get; private set; }

        public MainWindow()
        {
            this.InitializeComponent();

            Setup();
        }

        public void Setup()
        {
            AppWindow.Resize(new Windows.Graphics.SizeInt32 { Height = 800, Width = 1000 });
            AppWindow.SetIcon(@"Assets/Icon/sbruhhhtify.ico");
            AppWindow.TitleBar.BackgroundColor = Windows.UI.Color.FromArgb(255, 41, 41, 41);
            AppWindow.TitleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(255, 41, 41, 41);
            Application.Current.Resources["ButtonBackgroundPointerOver"] = Windows.UI.Color.FromArgb(255, 48, 48, 48);
            DisableResize();

            mainView = this;
            mainVM = new MainViewModel();
            MainPage.DataContext = mainVM;
        }

        // Disable Resize for User
        // https://github.com/microsoft/WindowsAppSDK/discussions/1694
        public void DisableResize()
        {
            var windowhandle = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(windowhandle);
            var appwindow = AppWindow.GetFromWindowId(windowId);
            var presenter = appwindow.Presenter as OverlappedPresenter;

            presenter.IsResizable = false;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
        }
    }
}
