using Windows.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Tetris.Pages
{
    public sealed partial class PiecePreviewUserControl : UserControl
    {

        public readonly int PREVIEW_GRID_SIZE = 4;
        private readonly Border[,] borders;
        public PiecePreviewUserControl()
        {
            InitializeComponent();
            borders = new Border[PREVIEW_GRID_SIZE, PREVIEW_GRID_SIZE];
            Grid previewGrid = FindName("PreviewGrid") as Grid;
            for (int i = 0; i < PREVIEW_GRID_SIZE; i++)
            {
                previewGrid.ColumnDefinitions.Add(new ColumnDefinition());
                for (int j = 0; j < PREVIEW_GRID_SIZE; j++)
                {
                    if (i == 0)
                    {
                        previewGrid.RowDefinitions.Add(new RowDefinition());
                    }
                    Border border = new Border();
                    previewGrid.Children.Add(border);
                    Grid.SetColumn(border, i);
                    Grid.SetRow(border, j);
                    //WPF grids are top to bottom, but we want to store coodinates bottom to top
                    borders[i, PREVIEW_GRID_SIZE - j - 1] = border;
                }
            }
        }

        public Border GetBorder(int x, int y)
        {
            if (x < borders.GetLength(0) || y < borders.GetLength(1))
            {
                return borders[x, y];
            }
            return null;
        }
    }
}
