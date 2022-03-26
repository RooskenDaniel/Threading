using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Tetris.Pages

{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainMenuPage : Page
    {
        private int? FocusedRow = 0;
        public MainMenuPage()
        {
            this.InitializeComponent();
        }

        void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(GamePage));
        }

        void ReplayButton_Click(object sender, RoutedEventArgs e)
        {

        }

        void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Exit();
        }

        void FullscreenButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            if (view.IsFullScreenMode)
            {
                view.ExitFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.Maximized;
            }
            else
            {
                view.TryEnterFullScreenMode();
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;
            }
        }

        private void Page_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            int newValue;
            switch (e.Key)
            {
                case VirtualKey.S:
                case VirtualKey.Down:
                case VirtualKey.GamepadDPadDown:
                case VirtualKey.GamepadLeftThumbstickDown:
                    UniformGrid grid = this.FindName("ButtonGrid") as UniformGrid;
                    if (FocusedRow != null)
                    {
                        newValue = FocusedRow.Value + 1;
                        if (newValue < grid.Rows)
                        {
                            FocusedRow = newValue;
                        }
                    }
                    setFocus();
                    break;
                case VirtualKey.W:
                case VirtualKey.Up:
                case VirtualKey.GamepadDPadUp:
                case VirtualKey.GamepadLeftThumbstickUp:
                    if (FocusedRow != null)
                    {
                        newValue = FocusedRow.Value - 1;
                        if (newValue >= 0)
                        {
                            FocusedRow = newValue;
                        }
                    }
                    setFocus();
                    break;
                default: break;
            }
        }

        private void setFocus()
        {
            UniformGrid grid = this.FindName("ButtonGrid") as UniformGrid;
            if (FocusedRow == null)
            {
                FocusedRow = 0;
            }
            Button b = grid.Children[FocusedRow.Value] as Button;
            b.Focus(FocusState.Keyboard);
        }
    }
}
