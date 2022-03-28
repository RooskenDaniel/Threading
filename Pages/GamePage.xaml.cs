using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
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
    public sealed partial class GamePage : Page
    {
        private const int PLAYFIELD_HEIGHT = 20;
        private const int PLAYFIELD_WIDTH = 10;
        private const int MIN_LOOP_TIME_MS = 7;
        private const double SIDE_BAR_RATIO = 0.5;
        bool gameLoopActive = true;
        private readonly Border[,] borders;

        private readonly PlayField playField;
        private double gameTime = 0;
        public GamePage()
        {
            this.InitializeComponent();
            //set the inital playing field size
            setMidCellSize();

            //set sidebar size relative to the playfield
            ColumnDefinition sideBarCol = this.FindName("sideBarColumn") as ColumnDefinition;
            sideBarCol.Width = new GridLength(SIDE_BAR_RATIO, GridUnitType.Star);

            //create the playfield borders and model
            borders = new Border[PLAYFIELD_WIDTH, PLAYFIELD_HEIGHT];
            AddPlayFieldBorders(PLAYFIELD_WIDTH, PLAYFIELD_HEIGHT);
            playField = new PlayField(PLAYFIELD_WIDTH, PLAYFIELD_HEIGHT);

            //register for inputs
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;

            //start the gameloop/logic thread
            //keeps UI responsive during the game loop
            new Thread(GameLoop).Start();
        }
        #region Initialization helpers
        private void AddPlayFieldBorders(int width, int height)
        {
            Grid playArea = this.FindName("PlayAreaGrid") as Grid;
            for (int i = 0; i < width; i++)
            {
                playArea.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < height; j++)
                {
                    Border border = new Border();
                    border.BorderThickness = new Thickness(1, 1, 1, 1);
                    border.BorderBrush = new SolidColorBrush(Windows.UI.Colors.DimGray);
                    playArea.Children.Add(border);
                    Grid.SetColumn(border, i);
                    Grid.SetRow(border, j);
                    //WPF grids are top to bottom, but we want to store coodinates bottom to top
                    borders[i, height - j - 1] = border;
                }
            }
            for (int i = 0; i < height; i++)
            {
                playArea.RowDefinitions.Add(new RowDefinition());
            }
        }

        private void SetBorderColor(int x, int y, CellState cellState)
        {
            Border b = borders[x, y];
            switch (cellState)
            {
                case CellState.LIGHT_BLUE:
                    b.Background = new SolidColorBrush(Windows.UI.Colors.Aqua);
                    break;
                case CellState.DARK_BLUE:
                    b.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
                    break;
                case CellState.ORANGE:
                    b.Background = new SolidColorBrush(Windows.UI.Colors.Orange);
                    break;
                case CellState.YELLOW:
                    b.Background = new SolidColorBrush(Windows.UI.Colors.Yellow);
                    break;
                case CellState.GREEN:
                    b.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
                    break;
                case CellState.RED:
                    b.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
                case CellState.MAGENTA:
                    b.Background = new SolidColorBrush(Windows.UI.Colors.DarkViolet);
                    break;
                default:
                    b.Background = null;
                    break;
            }
        }
        #endregion
        #region Game loop
        private void GameLoop()
        {
            while (gameLoopActive)
            {
                long start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                gameTime++;
                //update game objects with the logic thread
                Tick();
                //update page with main thread
                IAsyncAction action = CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, UpdatePage);
                //block until UI thread is done or else logic and UI will desync.
                action.AsTask().Wait();
                int wait = (int)(DateTimeOffset.Now.ToUnixTimeMilliseconds() + MIN_LOOP_TIME_MS - start);
                Thread.Sleep(wait);
            }
        }

        //runs on logic thread
        private void Tick()
        {
            playField.Tick(gameTime);
        }

        //runs on UI (main) thread
        private void UpdatePage()
        {
            for (int x = 0; x < PLAYFIELD_WIDTH; x++)
            {
                for (int y = 0; y < PLAYFIELD_HEIGHT; y++)
                {
                    SetBorderColor(x, y, playField.getCellAppearance(x, y));
                }
            }
        }
        #endregion


        void CoreWindow_KeyDown(Windows.UI.Core.CoreWindow sender, Windows.UI.Core.KeyEventArgs e)
        {
            {
                switch (e.VirtualKey)
                {
                    case VirtualKey.Escape:
                        if (Frame.CanGoBack)
                        {
                            Frame.GoBack();
                        }
                        else
                        {
                            Frame.Navigate(typeof(MainMenuPage));
                        }

                        break;
                    case VirtualKey.Left:
                    case VirtualKey.A:
                    case VirtualKey.GamepadDPadLeft:
                    case VirtualKey.GamepadLeftThumbstickLeft:
                        playField.MovePieceLeft();
                        break;
                    case VirtualKey.Right:
                    case VirtualKey.D:
                    case VirtualKey.GamepadDPadRight:
                    case VirtualKey.GamepadLeftThumbstickRight:
                        playField.MovePieceRight();
                        break;
                    case VirtualKey.Up:
                    case VirtualKey.W:
                    case VirtualKey.GamepadDPadUp:
                    case VirtualKey.GamepadLeftThumbstickUp:
                        playField.HardDrop();
                        break;
                    case VirtualKey.Down:
                    case VirtualKey.S:
                    case VirtualKey.GamepadDPadDown:
                    case VirtualKey.GamepadLeftThumbstickDown:
                        playField.SoftDrop();
                        break;
                    case VirtualKey.Q:
                    case VirtualKey.GamepadA:
                        playField.RotatePieceRight();
                        break;
                    case VirtualKey.E:
                    case VirtualKey.GamepadB:
                        playField.RotatePieceLeft();
                        break;
                    case VirtualKey.Space:
                    case VirtualKey.GamepadRightShoulder:
                        playField.HoldPiece();
                        break;
                }
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            setMidCellSize();
        }

        /// <summary>
        /// Makes the playing field responsive by reszing it while mantaining it's aspect ratio
        /// </summary>
        private void setMidCellSize()
        {
            RowDefinition midRow = this.FindName("midRow") as RowDefinition;
            ColumnDefinition midCol = this.FindName("midColumn") as ColumnDefinition;
            double ratio = (double)PLAYFIELD_HEIGHT / ((double)PLAYFIELD_WIDTH * (1 + SIDE_BAR_RATIO));
            double windowHeight = ((Frame)Window.Current.Content).ActualHeight;
            double windowWidth = ((Frame)Window.Current.Content).ActualWidth;
            if (windowHeight / ratio > windowWidth)
            {
                //use width
                midCol.Width = new GridLength(windowWidth, GridUnitType.Star);
                midRow.Height = new GridLength(windowWidth * ratio, GridUnitType.Pixel);

            }
            else
            {
                //use height
                midRow.Height = new GridLength(windowHeight, GridUnitType.Star);
                midCol.Width = new GridLength(windowHeight / ratio, GridUnitType.Pixel);
            }

        }
    }
}