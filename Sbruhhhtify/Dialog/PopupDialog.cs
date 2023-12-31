using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Popups;

namespace Sbruhhhtify.Dialog
{
    public class PopupDialog
    {
        public static async void CustomShow(string message, string title)
        {
            var windowhandle = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.MainView);
            MessageDialog msg = new MessageDialog(message);

            msg.Title = title;

            WinRT.Interop.InitializeWithWindow.Initialize(msg, windowhandle);
            await msg.ShowAsync();
        }

        public static async void ShowError(string message)
        {
            var windowhandle = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.MainView);
            MessageDialog msg = new MessageDialog(message);

            msg.Title = "Error";

            WinRT.Interop.InitializeWithWindow.Initialize(msg, windowhandle);
            await msg.ShowAsync();
        }
    }
}
