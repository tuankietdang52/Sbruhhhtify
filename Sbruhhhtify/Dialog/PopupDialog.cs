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
        public static async void Show(string message)
        {
            var windowhandle = WinRT.Interop.WindowNative.GetWindowHandle(MainWindow.MainView);
            MessageDialog msg = new MessageDialog(message);

            WinRT.Interop.InitializeWithWindow.Initialize(msg, windowhandle);
            await msg.ShowAsync();
        }
    }
}
