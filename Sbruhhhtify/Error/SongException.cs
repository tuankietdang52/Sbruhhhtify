using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Sbruhhhtify.Error
{
    public class SongException
    {
        public static async void IsAlreadyAdded(string message)
        {
            var windowhandle = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.MainView);
            MessageDialog msg = new MessageDialog("Song is already added\nError:" + message);

            WinRT.Interop.InitializeWithWindow.Initialize(msg, windowhandle);
            await msg.ShowAsync();
        }

        public static async void UnknowException(string message)
        {
            var windowhandle = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.MainView);
            MessageDialog msg = new MessageDialog("Error:" + message);

            WinRT.Interop.InitializeWithWindow.Initialize(msg, windowhandle);
            await msg.ShowAsync();
        }
    }
}
