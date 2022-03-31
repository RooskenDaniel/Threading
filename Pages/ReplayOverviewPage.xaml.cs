using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Tetris.Models;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Tetris.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ReplayOverviewPage : Page
    {

        private List<Replay> LoadedReplays = new List<Replay>();
        private List<Replay> selectedReplays = new List<Replay>();
        public ReplayOverviewPage()
        {
            this.InitializeComponent();
            ListView listView = FindName("ReplayListView") as ListView;
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            LoadedReplays.Add(new Replay("aaaaaa"));
            listView.ItemsSource = LoadedReplays;

        }

        private void loadFileButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void startSimSingleThreadButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ReplaySimPage));
        }

        private void startSimThreadPoolButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ReplaySimPage));
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            Replay replay = checkbox.DataContext as Replay;
            selectedReplays.Add((Replay)replay);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox checkbox = (CheckBox)sender;
            Replay replay = checkbox.DataContext as Replay;
            selectedReplays.Remove((Replay)replay);
        }

        private void ReplayViewButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            Replay replay = button.DataContext as Replay;
            Frame.Navigate(typeof(GamePage), replay);

        }
    }
}
