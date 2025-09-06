using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.IO;

namespace HeavenUsurperToolkit
{
    /// <summary>
    /// PatternGenerator.xaml 的交互逻辑
    /// </summary>
    public partial class PatternGenerator : Page, IResizablePage
    {
        public double DesiredWidth => 1281;
        public double DesiredHeight => 850;
        public ObservableCollection<SequenceItem> SequenceItems { get; set; }

        public PatternGenerator()
        {
            InitializeComponent();
            SequenceItems = new ObservableCollection<SequenceItem>();
            SequenceDataGrid.ItemsSource = SequenceItems;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateBack(new MainPage());
            }
        }

        private void AddProcessButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取输入参数
            double bpm = 0;
            int divide = 0;
            double startTime = 0;
            double endTime = 0;
            string note = string.Empty;
            double max = 0;
            double average = 0;
            int line = 0;
            int gap = 0;

            Dispatcher.Invoke(() =>
            {
                bpm = double.TryParse(BPMText.Text, out var b) ? b : 0;
                divide = int.TryParse(DivideText.Text, out var d) ? d : 0;
                startTime = double.TryParse(StartTimeText.Text, out var st) ? st : 0;
                endTime = double.TryParse(EndTimeText.Text, out var et) ? et : 0;
                note = NoteText.Text;
                max = double.TryParse(MaxText.Text, out var m) ? m : 0;
                average = double.TryParse(AverageText.Text, out var avg) ? avg : 0;
                line = int.TryParse(LineText.Text, out var l) ? l : 0;
                gap = int.TryParse(GapText.Text, out var g) ? g : 0;
            });

            bool randomDistribution = RandomDistribution.IsChecked ?? false;
            bool withDensity = WithDensity.IsChecked ?? false;

            // 创建新的 SequenceItem
            SequenceItem newItem = new SequenceItem()
            {
                BPM = bpm,
                Divide = divide,
                StartTime = startTime,
                EndTime = endTime,
                Note = note,
                RandomDistribution = randomDistribution,
                Max = max,
                Average = average,
                WithDensity = withDensity,
                Line = line,
                Gap = gap
            };

            // 检查是否已存在相同项，如果不存在则添加到集合中
            if (!SequenceItems.Contains(newItem))
            {
                SequenceItems.Add(newItem);
            }

            // 清空参数
            Dispatcher.Invoke(() =>
            {
                BPMText.Clear();
                DivideText.Clear();
                StartTimeText.Clear();
                EndTimeText.Clear();
                NoteText.Clear();
                RandomDistribution.IsChecked = false;
                MaxText.Clear();
                AverageText.Clear();
                WithDensity.IsChecked = false;
                LineText.Clear();
                GapText.Clear();
            });
        }

        private void BrowseFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "osu files (*.osu)|*.osu";
            if (openFileDialog.ShowDialog() == true)
            {
                Dispatcher.Invoke(() =>
                {
                    OutputFilePathText.Text = openFileDialog.FileName;
                });
            }
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // Helper.DragDrop(e, GeneratePattern);

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files != null && files.Length > 0)
                {
                    // 这里可以添加处理拖入文件的逻辑
                    Dispatcher.Invoke(() =>
                    {
                        OutputFilePathText.Text = files[0];
                    });
                    //MessageBox.Show($"文件已拖入: {files[0]}");
                }
            }
        }

        private void GeneratePattern(string fileName)
        {
            OsuFileV14 osu = new OsuFileV14(fileName);
            Random rand = new Random();
            var chartOffset = osu.TimingPoints.First().Time;
            var removed = osu.HitObjects.ToList();
            try
            {
                foreach (SequenceItem item in SequenceItems)
                {
                    // 移除所有StartTime在当前SequenceItem的时间范围内的HitObjects
                    removed.RemoveAll(hitObject =>
                        hitObject.StartTime >= item.StartTime &&
                        hitObject.StartTime <= item.EndTime);
                }

                foreach (SequenceItem item in SequenceItems)
                {
                    var noteList = item.Note.Trim().Split(' ').Select(n => int.Parse(n)).ToList();
                    var time = item.StartTime;
                    var gap = item.Gap + 1;
                    var lineArray = new int[gap];
                    while (time < item.EndTime)
                    {



                        time += 60000.0 / item.BPM / item.Divide;
                        
                        if (gap > 0)
                        {
                            for (int i = gap - 2; i >= 0; i--)
                            {
                                lineArray[i] = lineArray[i + 1];
                            }

                            // lineArray[gap - 1] = null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            // 用过滤后的HitObjects替换原来的列表
            // osu.HitObjects = removed;
        }

        private void ManualButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
            string outputFilePath = string.Empty;
            Dispatcher.Invoke(() =>
            {
                outputFilePath = OutputFilePathText.Text;
            });
            GeneratePattern(outputFilePath);
        }
    }

    public struct SequenceItem
    {
        public double BPM { get; set; }
        public int Divide { get; set; }
        public double StartTime { get; set; }
        public double EndTime { get; set; }
        public string Note { get; set; }
        public bool RandomDistribution { get; set; }
        public double Max { get; set; }
        public double Average { get; set; }
        public bool WithDensity { get; set; }
        public int Line { get; set; }
        public int Gap { get; set; }
    }
}
