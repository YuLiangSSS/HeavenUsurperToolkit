using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HeavenUsurperToolkit
{
    public partial class Page2 : Page, IResizablePage
    {
        public double DesiredWidth => 800;
        public double DesiredHeight => 850;
        private int resourceTypeCount = 3;
        private int processCount = 5;
        private int[] available;
        private int[,] max;
        private int[,] allocation;
        private int[,] need;
        private DataTable allocationTable;
        private DataTable needTable;
        private DataTable maxTable;

        // 定义三组不同的数据集
        private readonly DataSet1 dataSet1 = new DataSet1();
        private readonly DataSet2 dataSet2 = new DataSet2();
        private readonly DataSet3 dataSet3 = new DataSet3();

        public Page2()
        {
            InitializeComponent();
            InitializeDataSetSelector();
            available = new int[resourceTypeCount];
            need = new int[processCount, resourceTypeCount];
            max = new int[processCount, resourceTypeCount];
            allocation = new int[processCount, resourceTypeCount];
            allocationTable = new DataTable();
            needTable = new DataTable();
            maxTable = new DataTable();
        }

        private void InitializeDataSetSelector()
        {
            // 初始化数据集选择器
            DataSetSelector.Items.Add("数据集1 (3种资源)");
            DataSetSelector.Items.Add("数据集2 (3种资源)");
            DataSetSelector.Items.Add("数据集3 (4种资源)");
            // DataSetSelector.SelectedIndex = 0;
            
            // 加载默认数据集
            // LoadDataSet(0);
        }

        private void LoadDataSet(int dataSetIndex)
        {
            switch (dataSetIndex)
            {
                case 0:
                    LoadDataSet1();
                    break;
                case 1:
                    LoadDataSet2();
                    break;
                case 2:
                    LoadDataSet3();
                    break;
            }

            UpdateDataGrids();
            UpdateAvailableDisplay();
        }

        private void LoadDataSet1()
        {
            // 数据集1：3种资源
            resourceTypeCount = dataSet1.ResourceTypeCount;
            processCount = dataSet1.ProcessCount;
            
            // 初始化矩阵
            available = new int[resourceTypeCount];
            max = new int[processCount, resourceTypeCount];
            allocation = new int[processCount, resourceTypeCount];
            need = new int[processCount, resourceTypeCount];

            // 设置可用资源
            for (int i = 0; i < resourceTypeCount; i++)
            {
                available[i] = dataSet1.Available[i];
            }

            // 设置最大需求矩阵和已分配矩阵
            for (int i = 0; i < processCount; i++)
            {
                for (int j = 0; j < resourceTypeCount; j++)
                {
                    max[i, j] = dataSet1.Max[i, j];
                    allocation[i, j] = dataSet1.Allocation[i, j];
                    need[i, j] = max[i, j] - allocation[i, j];
                }
            }

            // 更新资源类型数量和进程数量的显示
            AvailableResourcesInput.Text = string.Join(",", dataSet1.Available);
        }

        private void LoadDataSet2()
        {
            // 数据集2：3种资源
            resourceTypeCount = dataSet2.ResourceTypeCount;
            processCount = dataSet2.ProcessCount;
            
            // 初始化矩阵
            available = new int[resourceTypeCount];
            max = new int[processCount, resourceTypeCount];
            allocation = new int[processCount, resourceTypeCount];
            need = new int[processCount, resourceTypeCount];

            // 设置可用资源
            for (int i = 0; i < resourceTypeCount; i++)
            {
                available[i] = dataSet2.Available[i];
            }

            // 设置最大需求矩阵和已分配矩阵
            for (int i = 0; i < processCount; i++)
            {
                for (int j = 0; j < resourceTypeCount; j++)
                {
                    max[i, j] = dataSet2.Max[i, j];
                    allocation[i, j] = dataSet2.Allocation[i, j];
                    need[i, j] = max[i, j] - allocation[i, j];
                }
            }

            // 更新资源类型数量和进程数量的显示
            AvailableResourcesInput.Text = string.Join(",", dataSet2.Available);
        }

        private void LoadDataSet3()
        {
            // 数据集3：4种资源
            resourceTypeCount = dataSet3.ResourceTypeCount;
            processCount = dataSet3.ProcessCount;
            
            // 初始化矩阵
            available = new int[resourceTypeCount];
            max = new int[processCount, resourceTypeCount];
            allocation = new int[processCount, resourceTypeCount];
            need = new int[processCount, resourceTypeCount];

            // 设置可用资源
            for (int i = 0; i < resourceTypeCount; i++)
            {
                available[i] = dataSet3.Available[i];
            }

            // 设置最大需求矩阵和已分配矩阵
            for (int i = 0; i < processCount; i++)
            {
                for (int j = 0; j < resourceTypeCount; j++)
                {
                    max[i, j] = dataSet3.Max[i, j];
                    allocation[i, j] = dataSet3.Allocation[i, j];
                    need[i, j] = max[i, j] - allocation[i, j];
                }
            }

            // 更新资源类型数量和进程数量的显示
            AvailableResourcesInput.Text = string.Join(",", dataSet3.Available);
        }

        // 数据集1：3种资源
        private class DataSet1
        {
            public int ResourceTypeCount => 3;
            public int ProcessCount => 5;
            public int[] Available => new int[] { 3, 3, 2 };
            public int[,] Max => new int[,] {
                { 7, 5, 3 },
                { 3, 2, 2 },
                { 9, 0, 2 },
                { 2, 2, 2 },
                { 4, 3, 3 }
            };
            public int[,] Allocation => new int[,] {
                { 0, 1, 0 },
                { 2, 0, 0 },
                { 3, 0, 2 },
                { 2, 1, 1 },
                { 0, 0, 2 }
            };
        }

        // 数据集2：3种资源
        private class DataSet2
        {
            public int ResourceTypeCount => 3;
            public int ProcessCount => 5;
            public int[] Available => new int[] { 1, 1, 1 }; // 较少的可用资源
            public int[,] Max => new int[,] {
                { 7, 5, 3 },
                { 3, 2, 2 },
                { 9, 1, 2 },
                { 2, 2, 2 },
                { 4, 3, 3 }
            };
            public int[,] Allocation => new int[,] {
                { 2, 2, 1 },
                { 2, 0, 1 },
                { 4, 1, 1 },
                { 2, 1, 1 },
                { 1, 0, 0 }
            };
        }

        // 数据集3：4种资源
        private class DataSet3
        {
            public int ResourceTypeCount => 4;
            public int ProcessCount => 4;
            public int[] Available => new int[] { 2, 3, 3, 2 };
            public int[,] Max => new int[,] {
                { 6, 4, 7, 3 },
                { 4, 5, 3, 2 },
                { 2, 3, 5, 4 },
                { 6, 3, 3, 2 }
            };
            public int[,] Allocation => new int[,] {
                { 1, 1, 2, 1 },
                { 2, 1, 1, 0 },
                { 1, 1, 1, 1 },
                { 2, 0, 1, 1 }
            };
        }

        private void DataSetSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataSetSelector.SelectedIndex >= 0)
            {
                LoadDataSet(DataSetSelector.SelectedIndex);
            }
        }

        private void SetResourceTypesButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ResourceTypesInput.Text, out int newResourceTypeCount) && newResourceTypeCount > 0)
            {
                resourceTypeCount = newResourceTypeCount;
                InitializeMatrices();
                MessageBox.Show($"资源类型数量已设置为 {resourceTypeCount}", "设置成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("请输入有效的资源类型数量（大于0的整数）", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SetProcessCountButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.TryParse(ProcessCountInput.Text, out int newProcessCount) && newProcessCount > 0)
            {
                processCount = newProcessCount;
                InitializeMatrices();
                MessageBox.Show($"进程数量已设置为 {processCount}", "设置成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("请输入有效的进程数量（大于0的整数）", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void SetAvailableButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] values = AvailableResourcesInput.Text.Split(',');
                if (values.Length != resourceTypeCount)
                {
                    MessageBox.Show($"请输入 {resourceTypeCount} 个资源数量，用逗号分隔", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                available = new int[resourceTypeCount];
                for (int i = 0; i < resourceTypeCount; i++)
                {
                    available[i] = int.Parse(values[i].Trim());
                }

                UpdateAvailableDisplay();
                MessageBox.Show("可用资源已更新", "设置成功", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                MessageBox.Show("输入格式错误，请输入有效的数字，用逗号分隔", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void InitializeMatrices()
        {
            available = new int[resourceTypeCount];
            max = new int[processCount, resourceTypeCount];
            allocation = new int[processCount, resourceTypeCount];
            need = new int[processCount, resourceTypeCount];

            UpdateDataGrids();
            UpdateAvailableDisplay();
        }

        private void UpdateDataGrids()
        {
            // 创建Max表
            maxTable = new DataTable();
            maxTable.Columns.Add("进程", typeof(string));
            for (int i = 0; i < resourceTypeCount; i++)
            {
                maxTable.Columns.Add($"R{i}", typeof(int));
            }

            for (int i = 0; i < processCount; i++)
            {
                DataRow row = maxTable.NewRow();
                row["进程"] = $"P{i}";
                for (int j = 0; j < resourceTypeCount; j++)
                {
                    row[$"R{j}"] = max[i, j];
                }
                maxTable.Rows.Add(row);
            }
            MaxGrid.ItemsSource = maxTable.DefaultView;

            // 创建Allocation表
            allocationTable = new DataTable();
            allocationTable.Columns.Add("进程", typeof(string));
            for (int i = 0; i < resourceTypeCount; i++)
            {
                allocationTable.Columns.Add($"R{i}", typeof(int));
            }

            for (int i = 0; i < processCount; i++)
            {
                DataRow row = allocationTable.NewRow();
                row["进程"] = $"P{i}";
                for (int j = 0; j < resourceTypeCount; j++)
                {
                    row[$"R{j}"] = allocation[i, j];
                }
                allocationTable.Rows.Add(row);
            }
            AllocationGrid.ItemsSource = allocationTable.DefaultView;

            // 创建Need表
            UpdateNeedMatrix();
        }

        private void UpdateNeedMatrix()
        {
            // 更新Need矩阵
            for (int i = 0; i < processCount; i++)
            {
                for (int j = 0; j < resourceTypeCount; j++)
                {
                    need[i, j] = max[i, j] - allocation[i, j];
                }
            }

            // 创建Need表
            needTable = new DataTable();
            needTable.Columns.Add("进程", typeof(string));
            for (int i = 0; i < resourceTypeCount; i++)
            {
                needTable.Columns.Add($"R{i}", typeof(int));
            }

            for (int i = 0; i < processCount; i++)
            {
                DataRow row = needTable.NewRow();
                row["进程"] = $"P{i}";
                for (int j = 0; j < resourceTypeCount; j++)
                {
                    row[$"R{j}"] = need[i, j];
                }
                needTable.Rows.Add(row);
            }
            NeedGrid.ItemsSource = needTable.DefaultView;
        }

        private void UpdateAvailableDisplay()
        {
            if (available != null)
            {
                AvailableResourcesDisplay.Text = string.Join(", ", available);
            }
        }

        private void ClearDataButton_Click(object sender, RoutedEventArgs e)
        {
            // 清空所有矩阵
            for (int i = 0; i < processCount; i++)
            {
                for (int j = 0; j < resourceTypeCount; j++)
                {
                    max[i, j] = 0;
                    allocation[i, j] = 0;
                    need[i, j] = 0;
                }
            }

            for (int i = 0; i < resourceTypeCount; i++)
            {
                available[i] = 0;
            }

            UpdateDataGrids();
            UpdateAvailableDisplay();
            SafetyResultText.Text = "数据已清空";
            SafeSequenceText.Text = "";
            RequestResultText.Text = "请重新设置数据";

            MessageBox.Show("所有数据已清空", "清空完成", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void CheckSafetyButton_Click(object sender, RoutedEventArgs e)
        {
            // 从DataGrid获取最新数据
            UpdateMatricesFromDataGrids();

            var result = CheckSafety();
            if (result.IsSafe)
            {
                SafetyResultText.Text = "系统处于安全状态";
                SafetyResultText.Foreground = new SolidColorBrush(Colors.Green);
                SafeSequenceText.Text = string.Join(" → ", result.SafeSequence.Select(p => $"P{p}"));
            }
            else
            {
                SafetyResultText.Text = "系统处于不安全状态";
                SafetyResultText.Foreground = new SolidColorBrush(Colors.Red);
                SafeSequenceText.Text = "无安全序列";
            }
        }

        private void UpdateMatricesFromDataGrids()
        {
            // 从MaxGrid更新max矩阵
            for (int i = 0; i < processCount; i++)
            {
                DataRowView row = (DataRowView)MaxGrid.Items[i];
                for (int j = 0; j < resourceTypeCount; j++)
                {
                    max[i, j] = Convert.ToInt32(row[$"R{j}"]);
                }
            }

            // 从AllocationGrid更新allocation矩阵
            for (int i = 0; i < processCount; i++)
            {
                DataRowView row = (DataRowView)AllocationGrid.Items[i];
                for (int j = 0; j < resourceTypeCount; j++)
                {
                    allocation[i, j] = Convert.ToInt32(row[$"R{j}"]);
                }
            }

            // 更新need矩阵
            UpdateNeedMatrix();
        }

        private (bool IsSafe, List<int> SafeSequence) CheckSafety()
        {
            // 银行家算法安全性检查
            int[] work = new int[resourceTypeCount];
            bool[] finish = new bool[processCount];
            List<int> safeSequence = new List<int>();

            // 初始化work = available
            for (int i = 0; i < resourceTypeCount; i++)
            {
                work[i] = available[i];
            }

            // 初始化finish数组
            for (int i = 0; i < processCount; i++)
            {
                finish[i] = false;
            }

            int count = 0;
            while (count < processCount)
            {
                bool found = false;
                for (int i = 0; i < processCount; i++)
                {
                    if (!finish[i])
                    {
                        // 检查Need[i] <= Work
                        bool canAllocate = true;
                        for (int j = 0; j < resourceTypeCount; j++)
                        {
                            if (need[i, j] > work[j])
                            {
                                canAllocate = false;
                                break;
                            }
                        }

                        if (canAllocate)
                        {
                            // 分配资源
                            for (int j = 0; j < resourceTypeCount; j++)
                            {
                                work[j] += allocation[i, j];
                            }
                            finish[i] = true;
                            safeSequence.Add(i);
                            found = true;
                            count++;
                            break;
                        }
                    }
                }

                if (!found)
                {
                    return (false, new List<int>());
                }
            }

            return (true, safeSequence);
        }

        private void RequestResourcesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(ProcessSelector.Text, out int processId) || processId < 0 || processId >= processCount)
                {
                    MessageBox.Show($"请输入有效的进程ID (0-{processCount - 1})", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                string[] requestValues = RequestResourcesInput.Text.Split(',');
                if (requestValues.Length != resourceTypeCount)
                {
                    MessageBox.Show($"请输入 {resourceTypeCount} 个资源请求数量，用逗号分隔", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                int[] request = new int[resourceTypeCount];
                for (int i = 0; i < resourceTypeCount; i++)
                {
                    request[i] = int.Parse(requestValues[i].Trim());
                }

                // 从DataGrid获取最新数据
                UpdateMatricesFromDataGrids();

                var result = ProcessResourceRequest(processId, request);
                RequestResultText.Text = result.Message;
                RequestResultText.Foreground = result.IsSuccess ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);

                if (result.IsSuccess)
                {
                    UpdateDataGrids();
                    UpdateAvailableDisplay();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("输入格式错误，请检查输入", "输入错误", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private (bool IsSuccess, string Message) ProcessResourceRequest(int processId, int[] request)
        {
            // 检查请求是否超过需要量
            for (int i = 0; i < resourceTypeCount; i++)
            {
                if (request[i] > need[processId, i])
                {
                    return (false, $"进程P{processId}请求的资源R{i}数量({request[i]})超过了其需要量({need[processId, i]})");
                }
            }

            // 检查请求是否超过可用资源
            for (int i = 0; i < resourceTypeCount; i++)
            {
                if (request[i] > available[i])
                {
                    return (false, $"系统可用资源R{i}({available[i]})不足，无法满足请求({request[i]})");
                }
            }

            // 试探性分配
            int[] originalAvailable = new int[resourceTypeCount];
            int[,] originalAllocation = new int[processCount, resourceTypeCount];
            int[,] originalNeed = new int[processCount, resourceTypeCount];

            // 保存原始状态
            for (int i = 0; i < resourceTypeCount; i++)
            {
                originalAvailable[i] = available[i];
            }
            for (int i = 0; i < processCount; i++)
            {
                for (int j = 0; j < resourceTypeCount; j++)
                {
                    originalAllocation[i, j] = allocation[i, j];
                    originalNeed[i, j] = need[i, j];
                }
            }

            // 试探性分配资源
            for (int i = 0; i < resourceTypeCount; i++)
            {
                available[i] -= request[i];
                allocation[processId, i] += request[i];
                need[processId, i] -= request[i];
            }

            // 检查安全性
            var safetyResult = CheckSafety();
            if (safetyResult.IsSafe)
            {
                string requestStr = string.Join(", ", request);
                string sequenceStr = string.Join(" → ", safetyResult.SafeSequence.Select(p => $"P{p}"));
                return (true, $"资源分配成功！\n进程P{processId}获得资源[{requestStr}]\n系统仍处于安全状态\n安全序列：{sequenceStr}");
            }
            else
            {
                // 恢复原始状态
                for (int i = 0; i < resourceTypeCount; i++)
                {
                    available[i] = originalAvailable[i];
                }
                for (int i = 0; i < processCount; i++)
                {
                    for (int j = 0; j < resourceTypeCount; j++)
                    {
                        allocation[i, j] = originalAllocation[i, j];
                        need[i, j] = originalNeed[i, j];
                    }
                }

                string requestStr = string.Join(", ", request);
                return (false, $"资源分配被拒绝！\n进程P{processId}请求资源[{requestStr}]会导致系统进入不安全状态");
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateBack(new MainPage());
            }
        }
    }
}