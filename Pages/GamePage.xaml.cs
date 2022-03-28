using Microsoft.Toolkit.Uwp.UI.Controls;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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

//todo:
//Clean code + convert strings to resources
//Move gameloop / threads
//Game over state

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
        private const double PREVIEW_BARS_RATIO = 0.25;
        private const double STAT_BAR_RATIO = 0.5;
        bool gameLoopActive = true;
        private readonly Border[,] playFieldBorders;
        private readonly PiecePreviewUserControl heldPiecePreview;
        private readonly List<PiecePreviewUserControl> upcomingPiecesPreviews;

        private readonly PlayField playField;
        private double gameTime = 0;
        public GamePage()
        {
            this.InitializeComponent();
            //set the inital playing field size
            setMidCellSize();

            //create the playfield borders and model
            playFieldBorders = new Border[PLAYFIELD_WIDTH, PLAYFIELD_HEIGHT];
            AddPlayFieldBorders(PLAYFIELD_WIDTH, PLAYFIELD_HEIGHT);
            playField = new PlayField(PLAYFIELD_WIDTH, PLAYFIELD_HEIGHT);

            //get references to the previews
            heldPiecePreview = this.FindName("previewHeld") as PiecePreviewUserControl;
            StackPanel panel = this.FindName("upcomingPreviewsPanel") as StackPanel;
            upcomingPiecesPreviews = new List<PiecePreviewUserControl>();
            foreach (Object child in panel.Children)
            {
                if(child is PiecePreviewUserControl)
                {
                    upcomingPiecesPreviews.Add(child as PiecePreviewUserControl);
                }
            }
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
                    if(i == 0)
                    {
                        playArea.RowDefinitions.Add(new RowDefinition());
                    }
                    Border border = new Border();
                    border.BorderThickness = new Thickness(1, 1, 1, 1);
                    border.BorderBrush = new SolidColorBrush(Windows.UI.Colors.DimGray);
                    playArea.Children.Add(border);
                    Grid.SetColumn(border, i);
                    Grid.SetRow(border, j);
                    //WPF grids are top to bottom, but we want to store coodinates bottom to top
                    playFieldBorders[i, height - j - 1] = border;
                }
            }
        }


        private void SetBorderColor(Border border, CellState cellState)
        {
            switch (cellState)
            {
                case CellState.LIGHT_BLUE:
                    border.Background = new SolidColorBrush(Windows.UI.Colors.Aqua);
                    break;
                case CellState.DARK_BLUE:
                    border.Background = new SolidColorBrush(Windows.UI.Colors.Blue);
                    break;
                case CellState.ORANGE:
                    border.Background = new SolidColorBrush(Windows.UI.Colors.Orange);
                    break;
                case CellState.YELLOW:
                    border.Background = new SolidColorBrush(Windows.UI.Colors.Yellow);
                    break;
                case CellState.GREEN:
                    border.Background = new SolidColorBrush(Windows.UI.Colors.Lime);
                    break;
                case CellState.RED:
                    border.Background = new SolidColorBrush(Windows.UI.Colors.Red);
                    break;
                case CellState.MAGENTA:
                    border.Background = new SolidColorBrush(Windows.UI.Colors.DarkViolet);
                    break;
                default:
                    border.Background = null;
                    break;
            }
        }
        #endregion

        //todo thread managment / gameloop should probably not be a pages responsibility
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
            //update playing field
            for (int x = 0; x < PLAYFIELD_WIDTH; x++)
            {
                for (int y = 0; y < PLAYFIELD_HEIGHT; y++)
                {
                    SetBorderColor(playFieldBorders[x, y], playField.getCellAppearance(x, y));
                }
            }
            //update previews
            ImmutableQueue<Piece> queue = playField.IncomingPieces;
            foreach(PiecePreviewUserControl preview in upcomingPiecesPreviews)
            {
                if(queue.Count() > 0)
                {
                    Piece piece = queue.Peek();
                    drawPreviewPiece(preview, piece);
                    queue = queue.Dequeue();
                }
            }
            //update held preview
            if(playField.HeldPiece != null)
            {
                PiecePreviewUserControl preview = this.FindName("previewHeld") as PiecePreviewUserControl;
                drawPreviewPiece(preview, playField.HeldPiece);
            }

        }

        private void drawPreviewPiece(PiecePreviewUserControl preview, Piece piece)
        {
            for (int x = 0; x < preview.PREVIEW_GRID_SIZE; x++)
            {
                for (int y = 0; y < preview.PREVIEW_GRID_SIZE; y++)
                {
                    SetBorderColor(preview.getBorder(x, y), piece.getCellAppearance(x, y));
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
                        Frame.Navigate(typeof(MainMenuPage));
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

            double ratio = (double)PLAYFIELD_HEIGHT / ((double)PLAYFIELD_WIDTH * ((PREVIEW_BARS_RATIO * 2) + STAT_BAR_RATIO + 1));
            double windowHeight = ((Frame)Window.Current.Content).ActualHeight;
            double windowWidth = ((Frame)Window.Current.Content).ActualWidth;
            if (windowHeight / ratio > windowWidth)
            {
                //use width
                midCol.Width = new GridLength(windowWidth, GridUnitType.Pixel);
                midRow.Height = new GridLength(windowWidth * ratio, GridUnitType.Pixel);

            }
            else
            {
                //use height
                midRow.Height = new GridLength(windowHeight, GridUnitType.Pixel);
                midCol.Width = new GridLength(windowHeight / ratio, GridUnitType.Pixel);
            }
        }

        //binding with ActualHeight or ActualWidth doesn't work in UWP
        //So we need to use a sizeChanged handler to mantain it's aspect ratio
        private void preview_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            PiecePreviewUserControl preview = sender as PiecePreviewUserControl;
            preview.Height = preview.ActualWidth;
        }
    }
}