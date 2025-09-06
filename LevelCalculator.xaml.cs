using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace HeavenUsurperToolkit
{
    /// <summary>
    /// LevelCalculator.xaml 的交互逻辑
    /// </summary>
    public partial class LevelCalculator : Page, IResizablePage
    {
        const double LOWER_BOUND = 2.76257856739498;
        const double UPPPER_BOUND = 10.5541834716376;

        public double DesiredWidth => 1281;
        public double DesiredHeight => 850;

        public ObservableCollection<MapInfo> MapList { get; set; }

        public LevelCalculator()
        {
            InitializeComponent();
            MapList = new ObservableCollection<MapInfo>();
            MapDataGrid.ItemsSource = MapList;
        }

        public struct MapInfo
        {
            public double StarRating { get; set; }
            public double Level { get; set; }
            public double OD { get; set; }
            public string Title { get; set; }
            public string Difficulty { get; set; }
            public string Artist { get; set; }
            public string Creator { get; set; }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateBack(new MainPage());
            }
        }

        private void MapDataGrid_Drop(object sender, DragEventArgs e)
        {
            ProcessDroppedFiles(e);
        }

        private void ProcessDroppedFiles(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            Task.Run(() =>
            {
                Helper.GetDroppedFiles(e, s => Path.GetExtension(s).ToLower() == ".osu", (filePath, count) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            OsuFileV14 osu = new OsuFileV14(filePath);
                            MapInfo mapInfo = new MapInfo()
                            {
                                StarRating = Math.Round(osu.StarRating, 3),
                                Level = Math.Round(CalculateLevel(osu.StarRating), 2),
                                OD = Math.Round(osu.General.OverallDifficulty, 1),
                                Title = osu.Metadata.Title,
                                Difficulty = osu.Metadata.Difficulty,
                                Artist = osu.Metadata.Artist,
                                Creator = osu.Metadata.Creator
                            };
                            MapList.Add(mapInfo);
                        }
                        catch (Exception ex)
                        {
#if DEBUG
                            Task.Run(() =>
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
                            });
#endif
                        }
                    });
                });
            });
        }

        public double CalculateLevel(double starRating)
        {
            if (starRating >= LOWER_BOUND && starRating <= UPPPER_BOUND)
            {
                return FittingFormula(starRating);
            }

            if (starRating < LOWER_BOUND && starRating > 0)
            {
                return 3.6198 * starRating;
            }

            if (starRating > UPPPER_BOUND && starRating < 12.3456789)
            {
                return (2.791 * starRating) + 0.5436;
            }

            return double.NaN;
        }

        public double FittingFormula(double x)
        {
            double y = 6.9615 * Math.Pow(Math.E, 0.1374 * x);
            return y;
        }

        private void RecalculateButton_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < MapList.Count; i++)
            {
                MapInfo mapInfo = MapList[i];
                mapInfo.Level = Math.Round(CalculateLevel(mapInfo.StarRating), 2);
                MapList[i] = mapInfo;
            }
        }
    }
}
