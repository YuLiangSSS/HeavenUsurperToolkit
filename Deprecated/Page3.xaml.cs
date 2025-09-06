using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HeavenUsurperToolkit
{
    public partial class Page3 : Page, IResizablePage
    {
        public double DesiredWidth => 800;
        public double DesiredHeight => 850;
        // 存储模拟过程中的每一步状态
        private ObservableCollection<SimulationStep> simulationSteps;
        // 当前内存中的页面块
        private List<int> memoryBlocks;
        // 用户输入或生成的页面访问序列
        private List<int> pageSequence;
        // 内存块的数量，默认为3
        private int memorySize = 3;
        // 缺页次数统计
        private int pageFaults = 0;
        // 总页面访问次数统计
        private int totalAccesses = 0;

        public Page3()
        {
            InitializeComponent();
            // 初始化模拟步骤的集合，并将其绑定到ListView
            simulationSteps = new ObservableCollection<SimulationStep>();
            SimulationProcessListView.ItemsSource = simulationSteps;
            // 初始化内存块和页面序列列表
            memoryBlocks = new List<int>();
            pageSequence = new List<int>();
            // 初始化内存可视化界面
            InitializeMemoryVisualization();
        }

        /// <summary>
        /// 初始化内存块的可视化表示。
        /// 根据 memorySize 创建相应数量的方块来代表内存块。
        /// </summary>
        private void InitializeMemoryVisualization()
        {
            MemoryVisualizationPanel.Children.Clear();
            for (int i = 0; i < memorySize; i++)
            {
                Border block = new Border
                {
                    Width = 80,
                    Height = 40,
                    BorderBrush = Brushes.Gray,
                    BorderThickness = new Thickness(2),
                    Margin = new Thickness(5),
                    Background = Brushes.LightGray
                };

                TextBlock text = new TextBlock
                {
                    Text = "空",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FontSize = 16,
                    FontWeight = FontWeights.Bold
                };

                block.Child = text;
                MemoryVisualizationPanel.Children.Add(block);
            }
        }

        /// <summary>
        /// 更新内存块的可视化表示。
        /// 根据当前 memoryBlocks 中的页面，更新界面上每个方块显示的内容和背景色。
        /// </summary>
        private void UpdateMemoryVisualization()
        {
            for (int i = 0; i < memorySize; i++)
            {
                if (MemoryVisualizationPanel.Children[i] is Border block)
                {
                    if (block.Child is TextBlock text)
                    {
                        if (i < memoryBlocks.Count)
                        {
                            text.Text = memoryBlocks[i].ToString();
                            block.Background = Brushes.LightBlue;
                        }
                        else
                        {
                            text.Text = "空";
                            block.Background = Brushes.LightGray;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// “设置内存块数量”按钮的点击事件处理程序。
        /// 根据用户输入的数值更新 memorySize，并重新初始化内存可视化界面。
        /// </summary>
        private void SetMemoryButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(MemoryBlocksInput.Text, out int size) && size > 0 && size <= 10)
            {
                memorySize = size;
                memoryBlocks.Clear();
                InitializeMemoryVisualization();
                MessageBox.Show($"内存块数量已设置为 {size}", "设置成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("请输入有效的内存块数量(1-10)", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// “生成随机序列”按钮的点击事件处理程序。
        /// 生成一个包含20个随机页面号（0-7）的序列，并显示在输入框中。
        /// </summary>
        private void GenerateSequenceButton_Click(object sender, RoutedEventArgs e)
        {
            Random random = new Random();
            List<int> sequence = new List<int>();
            for (int i = 0; i < 20; i++)
            {
                sequence.Add(random.Next(0, 8));
            }
            PageSequenceInput.Text = string.Join(",", sequence);
        }

        /// <summary>
        /// “添加单个页面”按钮的点击事件处理程序。
        /// 将用户输入的单个页面号添加到当前的页面序列中。
        /// </summary>
        private void AddPageButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(SinglePageInput.Text, out int page) && page >= 0)
            {
                pageSequence.Add(page);
                UpdateCurrentSequenceDisplay();
                SinglePageInput.Clear();
            }
            else
            {
                MessageBox.Show("请输入有效的页面号(非负整数)", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        /// <summary>
        /// 更新界面上显示的当前页面序列。
        /// </summary>
        private void UpdateCurrentSequenceDisplay()
        {
            CurrentSequenceDisplay.Text = string.Join(",", pageSequence);
            PageSequenceInput.Text = CurrentSequenceDisplay.Text;
        }

        /// <summary>
        /// “开始模拟”按钮的点击事件处理程序。
        /// 解析输入的页面序列，重置模拟状态，并根据用户选择的算法（FIFO 或 LRU）执行模拟。
        /// </summary>
        private void StartSimulationButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // 解析页面序列
                string[] pageStrings = PageSequenceInput.Text.Split(',');
                pageSequence = pageStrings.Select(s => int.Parse(s.Trim())).ToList();

                // 重置状态
                simulationSteps.Clear();
                memoryBlocks.Clear();
                pageFaults = 0;
                totalAccesses = 0;

                // 根据选择的算法进行模拟
                if (AlgorithmSelector.SelectedIndex == 0)
                {
                    SimulateFIFO();
                }
                else
                {
                    SimulateLRU();
                }

                UpdatePageFaultRateDisplay();
                UpdateCurrentSequenceDisplay();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"输入格式错误: {ex.Message}", "错误", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// 执行 FIFO (先进先出) 页面置换算法模拟。
        /// </summary>
        private void SimulateFIFO()
        {
            Queue<int> fifoQueue = new Queue<int>();

            for (int i = 0; i < pageSequence.Count; i++)
            {
                int currentPage = pageSequence[i];
                totalAccesses++;
                bool isPageFault = false;

                if (!memoryBlocks.Contains(currentPage))
                {
                    isPageFault = true;
                    pageFaults++;

                    if (memoryBlocks.Count < memorySize)
                    {
                        memoryBlocks.Add(currentPage);
                        fifoQueue.Enqueue(currentPage);
                    }
                    else
                    {
                        int removedPage = fifoQueue.Dequeue();
                        int index = memoryBlocks.IndexOf(removedPage);
                        memoryBlocks[index] = currentPage;
                        fifoQueue.Enqueue(currentPage);
                    }
                }

                simulationSteps.Add(new SimulationStep
                {
                    Step = i + 1,
                    PageAccessed = currentPage,
                    MemoryState = string.Join(",", memoryBlocks),
                    IsPageFault = isPageFault ? "是" : "否"
                });

                UpdateMemoryVisualization();
            }
        }

        /// <summary>
        /// 执行 LRU (最近最少使用) 页面置换算法模拟。
        /// </summary>
        private void SimulateLRU()
        {
            // 跟踪每个页面的最后访问时间
            Dictionary<int, int> lastUsed = new Dictionary<int, int>();

            for (int i = 0; i < pageSequence.Count; i++)
            {
                int currentPage = pageSequence[i];
                totalAccesses++;
                bool isPageFault = false;

                // 更新当前页面的访问时间
                lastUsed[currentPage] = i;

                if (!memoryBlocks.Contains(currentPage))
                {
                    isPageFault = true;
                    pageFaults++;

                    if (memoryBlocks.Count < memorySize)
                    {
                        memoryBlocks.Add(currentPage);
                    }
                    else
                    {
                        // 找到最久未使用的页面（访问时间最早的）
                        int lruPage = memoryBlocks.OrderBy(page => lastUsed.ContainsKey(page) ? lastUsed[page] : -1).First();
                        int index = memoryBlocks.IndexOf(lruPage);
                        memoryBlocks[index] = currentPage;
                    }
                }

                simulationSteps.Add(new SimulationStep
                {
                    Step = i + 1,
                    PageAccessed = currentPage,
                    MemoryState = string.Join(",", memoryBlocks),
                    IsPageFault = isPageFault ? "是" : "否"
                });

                UpdateMemoryVisualization();
            }
        }

        private void UpdatePageFaultRateDisplay()
        {
            double rate = totalAccesses > 0 ? (double)pageFaults / totalAccesses * 100 : 0;
            PageFaultRateDisplay.Text = $"缺页次数: {pageFaults}, 总访问次数: {totalAccesses}, 缺页率: {rate:F2}%";
        }

        private void ClearResultsButton_Click(object sender, RoutedEventArgs e)
        {
            simulationSteps.Clear();
            memoryBlocks.Clear();
            pageSequence.Clear();
            pageFaults = 0;
            totalAccesses = 0;

            InitializeMemoryVisualization();
            UpdatePageFaultRateDisplay();
            CurrentSequenceDisplay.Text = "";
            PageSequenceInput.Clear();
        }

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

    public class SimulationStep
    {
        public int Step { get; set; }
        public int PageAccessed { get; set; }
        public string? MemoryState { get; set; }
        public string? IsPageFault { get; set; }
    }
}