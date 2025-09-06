using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace HeavenUsurperToolkit
{
    // 进程类定义
    public class ProcessClass
    {
        public required string Name { get; set; }
        public int ArrivalTime { get; set; }
        public int ServiceTime { get; set; }
    }

    // 进程结果类定义
    public class ProcessResult
    {
        public required string Name { get; set; }
        public int ArrivalTime { get; set; }
        public int ServiceTime { get; set; }
        public int StartTime { get; set; }
        public int FinishTime { get; set; }
        public int TurnaroundTime { get; set; }
        public double WeightedTurnaroundTime { get; set; }
    }

    public partial class Page1 : Page, IResizablePage
    {
        public double DesiredWidth => 800;
        public double DesiredHeight => 850;
        // 进程数据集合
        private ObservableCollection<ProcessClass> processes;
        // 结果数据集合
        private ObservableCollection<ProcessResult> results;
        // 随机数生成器
        private Random random;

        public Page1()
        {
            InitializeComponent();

            // 初始化数据
            processes = new ObservableCollection<ProcessClass>();
            results = new ObservableCollection<ProcessResult>();
            random = new Random();

            // 绑定数据源
            ProcessListView.ItemsSource = processes;
            ResultListView.ItemsSource = results;

            // 添加窗口大小改变事件处理
            this.SizeChanged += Page1_SizeChanged;
        }

        private void Page1_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // 当页面大小改变时，重新绘制甘特图
            DrawGanttChart();
        }

        // 添加进程按钮点击事件
        private void AddProcessButton_Click(object sender, RoutedEventArgs e)
        {
            // 验证输入
            if (string.IsNullOrWhiteSpace(ProcessNameInput.Text))
            {
                MessageBox.Show("请输入进程名称！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(ArrivalTimeInput.Text, out int arrivalTime) || arrivalTime < 0)
            {
                MessageBox.Show("到达时间必须是非负整数！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(ServiceTimeInput.Text, out int serviceTime) || serviceTime <= 0)
            {
                MessageBox.Show("服务时间必须是正整数！", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 添加进程
            processes.Add(new ProcessClass
            {
                Name = ProcessNameInput.Text,
                ArrivalTime = arrivalTime,
                ServiceTime = serviceTime
            });

            // 清空输入框
            ProcessNameInput.Text = "";
            ArrivalTimeInput.Text = "";
            ServiceTimeInput.Text = "";
        }

        // 自动生成数据按钮点击事件
        private void GenerateDataButton_Click(object sender, RoutedEventArgs e)
        {
            // 清空现有数据
            processes.Clear();

            // 生成5个随机进程
            for (int i = 1; i <= 5; i++)
            {
                processes.Add(new ProcessClass
                {
                    Name = "P" + i,
                    ArrivalTime = random.Next(0, 10),
                    ServiceTime = random.Next(1, 10)
                });
            }
        }

        // 清空数据按钮点击事件
        private void ClearDataButton_Click(object sender, RoutedEventArgs e)
        {
            processes.Clear();
            ClearResults();
        }

        // 计算按钮点击事件
        private void CalculateButton_Click(object sender, RoutedEventArgs e)
        {
            if (processes.Count == 0)
            {
                MessageBox.Show("请先添加进程数据！", "数据错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 清空之前的结果
            ClearResults();

            // 根据选择的算法执行计算
            if (AlgorithmSelector.SelectedIndex == 0)
            {
                // FCFS算法
                CalculateFCFS();
            }
            else
            {
                // SJF算法
                CalculateSJF();
            }

            // 计算并显示平均值
            CalculateAverages();

            // 绘制甘特图
            DrawGanttChart();
        }

        // 先来先服务(FCFS)算法
        private void CalculateFCFS()
        {
            // 按到达时间排序
            var sortedProcesses = processes.OrderBy(p => p.ArrivalTime).ToList();

            int currentTime = 0;

            foreach (var process in sortedProcesses)
            {
                // 如果当前时间小于进程到达时间，则更新当前时间
                if (currentTime < process.ArrivalTime)
                {
                    currentTime = process.ArrivalTime;
                }

                int startTime = currentTime;
                int finishTime = startTime + process.ServiceTime;
                int turnaroundTime = finishTime - process.ArrivalTime;
                double weightedTurnaroundTime = (double)turnaroundTime / process.ServiceTime;

                // 添加结果
                results.Add(new ProcessResult
                {
                    Name = process.Name,
                    ArrivalTime = process.ArrivalTime,
                    ServiceTime = process.ServiceTime,
                    StartTime = startTime,
                    FinishTime = finishTime,
                    TurnaroundTime = turnaroundTime,
                    WeightedTurnaroundTime = Math.Round(weightedTurnaroundTime, 2)
                });

                // 更新当前时间
                currentTime = finishTime;
            }
        }

        // 短作业优先(SJF)算法
        private void CalculateSJF()
        {
            // 创建进程副本，避免修改原始数据
            var processList = processes.Select(p => new ProcessClass
            {
                Name = p.Name,
                ArrivalTime = p.ArrivalTime,
                ServiceTime = p.ServiceTime
            }).ToList();

            int currentTime = 0;

            // 找到最早到达的进程的时间
            int earliestArrival = processList.Min(p => p.ArrivalTime);
            currentTime = earliestArrival;

            while (processList.Count > 0)
            {
                // 找出已到达的进程
                var arrivedProcesses = processList.Where(p => p.ArrivalTime <= currentTime).ToList();

                if (arrivedProcesses.Count == 0)
                {
                    // 如果没有进程到达，将时间推进到下一个到达的进程
                    int nextArrival = processList.Min(p => p.ArrivalTime);
                    currentTime = nextArrival;
                    continue;
                }

                // 在已到达的进程中选择服务时间最短的
                var shortestJob = arrivedProcesses.OrderBy(p => p.ServiceTime).First();

                int startTime = currentTime;
                int finishTime = startTime + shortestJob.ServiceTime;
                int turnaroundTime = finishTime - shortestJob.ArrivalTime;
                double weightedTurnaroundTime = (double)turnaroundTime / shortestJob.ServiceTime;

                // 添加结果
                results.Add(new ProcessResult
                {
                    Name = shortestJob.Name,
                    ArrivalTime = shortestJob.ArrivalTime,
                    ServiceTime = shortestJob.ServiceTime,
                    StartTime = startTime,
                    FinishTime = finishTime,
                    TurnaroundTime = turnaroundTime,
                    WeightedTurnaroundTime = Math.Round(weightedTurnaroundTime, 2)
                });

                // 更新当前时间
                currentTime = finishTime;

                // 从列表中移除已处理的进程
                processList.Remove(shortestJob);
            }
        }

        // 计算平均周转时间和平均带权周转时间
        private void CalculateAverages()
        {
            if (results.Count == 0) return;

            double avgTurnaroundTime = results.Average(r => r.TurnaroundTime);
            double avgWeightedTurnaroundTime = results.Average(r => r.WeightedTurnaroundTime);

            AvgTurnaroundTimeText.Text = $"平均周转时间：{Math.Round(avgTurnaroundTime, 2)}";
            AvgWeightedTurnaroundTimeText.Text = $"平均带权周转时间：{Math.Round(avgWeightedTurnaroundTime, 2)}";
        }

        // 清空结果方法（在代码中被调用但未定义）
        private void ClearResults()
        {
            results.Clear();
            AvgTurnaroundTimeText.Text = "平均周转时间：";
            AvgWeightedTurnaroundTimeText.Text = "平均带权周转时间：";
            GanttChartCanvas.Children.Clear();
        }

        // 绘制甘特图方法
        private void DrawGanttChart()
        {
            GanttChartCanvas.Children.Clear();

            if (results.Count == 0) return;

            // 计算甘特图的时间范围
            int minTime = results.Min(r => r.StartTime);
            int maxTime = results.Max(r => r.FinishTime);

            // 设置比例尺
            double canvasWidth = GanttChartCanvas.ActualWidth > 0 ? GanttChartCanvas.ActualWidth : 300;
            double timeScale = canvasWidth / (maxTime - minTime);
            double blockHeight = 50;

            // 绘制时间轴
            Line timeline = new Line
            {
                X1 = 0,
                Y1 = 80,
                X2 = canvasWidth,
                Y2 = 80,
                Stroke = Brushes.Black,
                StrokeThickness = 1
            };
            GanttChartCanvas.Children.Add(timeline);

            // 绘制时间刻度
            for (int t = minTime; t <= maxTime; t++)
            {
                double x = (t - minTime) * timeScale;

                Line tickMark = new Line
                {
                    X1 = x,
                    Y1 = 75,
                    X2 = x,
                    Y2 = 85,
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                GanttChartCanvas.Children.Add(tickMark);

                TextBlock timeLabel = new TextBlock
                {
                    Text = t.ToString(),
                    FontSize = 10
                };
                Canvas.SetLeft(timeLabel, x - 5);
                Canvas.SetTop(timeLabel, 85);
                GanttChartCanvas.Children.Add(timeLabel);
            }

            // 绘制进程块
            Random colorRandom = new Random(42); // 固定种子以获得一致的颜色
            Dictionary<string, SolidColorBrush> processColors = new Dictionary<string, SolidColorBrush>();

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                // 为每个进程分配一个颜色
                if (!processColors.ContainsKey(result.Name))
                {
                    byte r = (byte)colorRandom.Next(100, 240);
                    byte g = (byte)colorRandom.Next(100, 240);
                    byte b = (byte)colorRandom.Next(100, 240);
                    processColors[result.Name] = new SolidColorBrush(Color.FromRgb(r, g, b));
                }

                double x = (result.StartTime - minTime) * timeScale;
                double width = result.ServiceTime * timeScale;
                double y = 20;

                // 绘制进程块
                Rectangle processBlock = new Rectangle
                {
                    Width = width,
                    Height = blockHeight,
                    Fill = processColors[result.Name],
                    Stroke = Brushes.Black,
                    StrokeThickness = 1
                };
                Canvas.SetLeft(processBlock, x);
                Canvas.SetTop(processBlock, y);
                GanttChartCanvas.Children.Add(processBlock);

                // 添加进程名称标签
                TextBlock nameLabel = new TextBlock
                {
                    Text = result.Name,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.Black
                };
                Canvas.SetLeft(nameLabel, x + (width / 2) - 10);
                Canvas.SetTop(nameLabel, y + (blockHeight / 2) - 10);
                GanttChartCanvas.Children.Add(nameLabel);

                // 添加时间标签
                TextBlock timeInfoLabel = new TextBlock
                {
                    Text = $"{result.StartTime}-{result.FinishTime}",
                    FontSize = 10
                };
                Canvas.SetLeft(timeInfoLabel, x + (width / 2) - 15);
                Canvas.SetTop(timeInfoLabel, y + (blockHeight / 2) + 10);
                GanttChartCanvas.Children.Add(timeInfoLabel);
            }
        }

        // 返回按钮点击事件处理
        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            // 获取主窗口实例
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                // 调用主窗口的导航方法
                mainWindow.NavigateBack(new MainPage());
            }
        }
    }
}