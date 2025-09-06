using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace HeavenUsurperToolkit
{
    public partial class KeyConverter : Page, IResizablePage
    {
        public double DesiredWidth => 800;
        public double DesiredHeight => 850;

        public const double INTERVAL = 50;

        public const double LN_INTERVAL = 10;

        public const double ERROR = 1.5;

        public const int BlankColumn = 1;

        private int totalFiles = 0;
        private int processedFiles = 0;

        public KeyConverter()
        {
            InitializeComponent();
            ProgressStackPanel.Visibility = Visibility.Collapsed;
        }


        private void UpdateProgress(bool canBeConverted = true, bool increment = true)
        {
            Dispatcher.Invoke(() =>
            {
                if (!canBeConverted)
                {
                    totalFiles--;
                    processedFiles--;
                }

                if (totalFiles <= 0)
                {
                    return;
                }

                if (increment)
                {
                    processedFiles++;
                }

                ConversionProgress.Value = (double)processedFiles / totalFiles * 100;
                ProgressText.Text = $"{processedFiles}/{totalFiles}";
            });
        }

        private void ProcessDroppedFiles(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            Task.Run(() =>
            {
                Dispatcher.Invoke(() =>
                {
                    processedFiles = 0;
                    ConversionProgress.Value = 0;
                });

                var allFiles = Helper.GetDroppedFiles(e, s => Path.GetExtension(s).ToLower() == ".osu", (filePath, count) =>
                 {
                     Dispatcher.Invoke(() =>
                     {
                         totalFiles += count;
                         ProgressStackPanel.Visibility = Visibility.Visible;
                         UpdateProgress(true, false);
                     });
                 });

                totalFiles = allFiles.Count;

                if (totalFiles > 0)
                {
                    string seedText = string.Empty;
                    double blank = 0.0;
                    double clean = 0.0;
                    double gap = 0.0;
                    double key = 0.0;
                    string creator = string.Empty;
                    bool creatorIsChecked = false;
                    List<int> checkKeys = new List<int>();

                    Dispatcher.Invoke(() =>
                    {
                        seedText = SeedText.Text;
                        blank = BlankValue.Value;
                        clean = CleanValue.Value;
                        gap = GapValue.Value;
                        key = KeyValue.Value;
                        creator = CreatorText.Text;
                        creatorIsChecked = CreatorCheckBox.IsChecked == true;

                        var keyCheckBoxes = new Dictionary<int, CheckBox>()
                        {
                            {1, C1k},
                            {2, C2k},
                            {3, C3k},
                            {4, C4k},
                            {5, C5k},
                            {6, C6k},
                            {7, C7k},
                            {8, C8k},
                            {9, C9k}
                        };

                        foreach (var kvp in keyCheckBoxes)
                        {
                            if (kvp.Value.IsChecked == true)
                            {
                                checkKeys.Add(kvp.Key);
                            }
                        }
                    });

                    foreach (var file in allFiles)
                    {
                        try
                        {
                            ApplyToBeatmap(file, seedText, blank, clean, gap, key, creator, creatorIsChecked, checkKeys);
                        }
                        catch (Exception ex)
                        {
                            UpdateProgress(false);

#if DEBUG
                            Task.Run(() =>
                            {
                                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
                            });
#endif
                        }
                    }
                }
            });
        }

        private void Rectangle_Drop(object sender, DragEventArgs e)
        {
            ProcessDroppedFiles(e);
        }

        private void TextBlock_Drop(object sender, DragEventArgs e)
        {
            ProcessDroppedFiles(e);
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateBack(new MainPage());
            }
        }

        public void ApplyToBeatmap(string name, string seedText, double blankValue, double cleanValue, double gapValue, double keyValue, string creator, bool creatorIsChecked, List<int> checkKeys)
        {
            bool canBeConverted = true;
            if (Path.GetExtension(name) != ".osu")
            {
                canBeConverted = false;
            }

            OsuFileV14 osu = OsuFileProcessor.ReadFile(name);

            if (osu.General.Mode != "3")
            {
                canBeConverted = false;
            }

            int seed;
            if (string.IsNullOrEmpty(seedText))
            {
                seed = Guid.NewGuid().GetHashCode();
            }
            else
            {
                {
                    byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(seedText));
                    seed = BitConverter.ToInt32(bytes, 0);
                }
            }
            var Rng = new Random(seed);

            int keys = (int)osu.General.CircleSize;
            int blank = (int)blankValue;

            if (keys > 9 || (int)keyValue <= keys)
            {
                canBeConverted = false;
            }

            if (blank > (int)keyValue - keys)
            {
                blank = (int)keyValue - keys;
            }

            if (checkKeys.Count > 0 && !checkKeys.Contains(keys))
            {
                canBeConverted = false;
            }


            if (!canBeConverted)
            {
                UpdateProgress(false);
                return;
            }


            osu.ManiaToKeys((int)keyValue);

            var newObjects = new List<ManiaHitObject>();

            var locations = osu.HitObjects.OrderBy(h => h.StartTime).ToList();

            List<ManiaHitObject> area = new List<ManiaHitObject>();
            List<ManiaHitObject> checkList = new List<ManiaHitObject>();

            var tempObjects = locations.OrderBy(h => h.StartTime).ToList();

            int sumTime = 0;
            int lastTime = 0;

            foreach (var timingPoint in tempObjects.GroupBy(h => h.StartTime))
            {
                var newLocations = timingPoint.OfType<ManiaHitObject>().Select(n => (n.Column, n.StartTime, n.EndTime)).OrderBy(h => h.Column).ToList();

                List<ManiaHitObject> line = new List<ManiaHitObject>();

                foreach (var note in newLocations)
                {
                    ManiaHitObject tempNote = new ManiaHitObject(note.Column, (int)keyValue, note.StartTime, endTime: note.EndTime);
                    line.Add(tempNote);
                }

                sumTime += timingPoint.Key - lastTime;
                lastTime = timingPoint.Key;

                area.AddRange(line);

                double gap = (29998.8584 * Math.Pow(Math.E, -0.3176 * gapValue)) + 347.7248;

                if (gapValue == 0)
                {
                    gap = double.MaxValue;
                }

                if (sumTime >= gap)
                {
                    sumTime = 0;
                    int cleanDivide = (int)cleanValue;
                    var processed = ProcessArea(osu, Rng, area, keys, (int)keyValue, blank, cleanDivide, ERROR, checkList);
                    newObjects.AddRange(processed.result);
                    checkList = processed.checkList.ToList();
                    area.Clear();
                }
            }

            if (area.Count > 0)
            {
                int cleanDivide = (int)cleanValue;
                var processed = ProcessArea(osu, Rng, area, keys, (int)keyValue, blank, cleanDivide, ERROR, checkList);
                newObjects.AddRange(processed.result);
            }

            newObjects = newObjects.OrderBy(h => h.StartTime).ToList();

            var cleanObjects = new List<ManiaHitObject>();

            foreach (var column in newObjects.GroupBy(h => h.Column))
            {
                var newColumnObjects = new List<ManiaHitObject>();

                var cleanLocations = column.OfType<ManiaHitObject>().Select(n => (startTime: n.StartTime, endTime: n.EndTime)).OrderBy(h => h.startTime).ToList();

                int lastStartTime = cleanLocations[0].startTime;
                int lastEndTime = cleanLocations[0].endTime;

                for (int i = 0; i < cleanLocations.Count; i++)
                {
                    if (i == 0)
                    {
                        lastStartTime = cleanLocations[0].startTime;
                        lastEndTime = cleanLocations[0].endTime;
                        continue;
                    }

                    if (cleanLocations[i].startTime >= lastStartTime && cleanLocations[i].startTime <= lastEndTime)
                    {
                        cleanLocations.RemoveAt(i);
                        i--;
                        continue;
                    } // if the note in a LN

                    if (Math.Abs(cleanLocations[i].startTime - lastStartTime) <= INTERVAL)
                    {
                        lastStartTime = cleanLocations[i].startTime;
                        lastEndTime = cleanLocations[i].endTime;
                        cleanLocations.RemoveAt(i);
                        i--;
                        continue;
                    } // interval judgement

                    if (Math.Abs(cleanLocations[i].startTime - lastEndTime) <= LN_INTERVAL)
                    {
                        lastStartTime = cleanLocations[i].startTime;
                        lastEndTime = cleanLocations[i].endTime;
                        cleanLocations.RemoveAt(i);
                        i--;
                        continue;
                    } // LN interval judgement

                    ManiaHitObject lastNote = new ManiaHitObject(column.Key, (int)keyValue, (int)lastStartTime, endTime: (int)lastEndTime);

                    newColumnObjects.Add(lastNote);
                    lastStartTime = cleanLocations[i].startTime;
                    lastEndTime = cleanLocations[i].endTime;
                }

                ManiaHitObject newNote = new ManiaHitObject(column.Key, (int)keyValue, (int)lastStartTime, endTime: (int)lastEndTime);

                newColumnObjects.Add(newNote);

                cleanObjects.AddRange(newColumnObjects);
            }

            osu.HitObjects = cleanObjects.OrderBy(h => h.StartTime).ToList();

            if (blank == 0)
            {
                osu.Metadata.Difficulty = osu.Metadata.Difficulty.Insert(0, $"[{keys}To{keyValue}C]");
            }
            else
            {
                osu.Metadata.Difficulty = osu.Metadata.Difficulty.Insert(0, $"[{keys}To{keyValue}C{blank}B]");
            }

            if (creatorIsChecked && !string.IsNullOrEmpty(creator))
            {
                osu.Metadata.Creator = creator;
            }
            else
            {
                osu.Metadata.Creator += " & KeyConverter";
            }

            osu.WriteFile();

            UpdateProgress();

            // DebugNoteSum(osu);
        }

        public (List<ManiaHitObject> result, List<ManiaHitObject> checkList) ProcessArea(OsuFileV14 osu, Random Rng, List<ManiaHitObject> hitObjects, int fromKeys, int toKeys, int blankNum = 0, int clean = 0, double error = 0, List<ManiaHitObject>? checkList = null)
        {
            List<ManiaHitObject> newObjects = new List<ManiaHitObject>();
            List<(int column, bool isBlank)> copyColumn = [];
            List<int> insertColumn = [];
            List<ManiaHitObject> checkColumn = [];
            bool isFirst = true;

            int num = toKeys - fromKeys;
            while (num > 0)
            {
                int copy = Rng.Next(fromKeys);
                if (!copyColumn.Contains((copy, false)))
                {
                    copyColumn.Add((copy, false));
                    num--;
                }
            }

            num = blankNum;
            while (num > 0)
            {
                int copy = Rng.Next(fromKeys);
                if (copyColumn.Contains((copy, false)) && !copyColumn.Contains((copy, true)))
                {
                    int index = copyColumn.IndexOf((copy, false));
                    copyColumn[index] = (copy, true);
                    num--;
                }
            }

            num = toKeys - fromKeys;
            while (num > 0)
            {
                int insert = Rng.Next(toKeys);
                if (!insertColumn.Contains(insert))
                {
                    insertColumn.Add(insert);
                    num--;
                }
            }
            insertColumn = insertColumn.OrderBy(c => c).ToList();

            foreach (var timingPoint in hitObjects.GroupBy(h => h.StartTime))
            {
                var locations = timingPoint.OfType<ManiaHitObject>().ToList();
                var tempObjects = new List<ManiaHitObject>();
                int length = copyColumn.Count;

                for (int i = 0; i < locations.Count; i++)
                {
                    int column = locations[i].Column;
                    for (int j = 0; j < length; j++)
                    {
                        if (column == copyColumn[j].column && !copyColumn[j].isBlank)
                        {
                            ManiaHitObject temp = locations[i];
                            temp.Column = insertColumn[j];
                            tempObjects.Add(temp);
                        }

                        if (locations[i].Column >= insertColumn[j])
                        {
                            var tempLocation = locations[i];
                            tempLocation.Column += 1;
                            locations[i] = tempLocation;
                        }
                    }
                    ManiaHitObject tempNote = locations[i];
                    tempObjects.Add(tempNote);
                }

                if (isFirst && checkList is not null && checkList.Count > 0 && clean > 0)
                {
                    var checkC = checkList.Select(h => h.Column).ToList();
                    var checkS = checkList.Select(h => h.StartTime).ToList();
                    for (int i = 0; i < tempObjects.Count; i++)
                    {
                        if (checkC.Contains(tempObjects[i].Column))
                        {
                            if (clean != 0)
                            {
                                double beatLength = osu.TimingPointAt(tempObjects[i].StartTime).BeatLength;
                                double timeDivide = beatLength / clean;
                                int index = checkC.IndexOf(tempObjects[i].Column);

                                if (tempObjects[i].StartTime - checkS[index] < timeDivide + error)
                                {
                                    tempObjects.RemoveAt(i);
                                    i--;
                                }
                            }
                            else
                            {
                                tempObjects.RemoveAt(i);
                                i--;
                            }
                        }
                    }
                    isFirst = false;
                }

                checkColumn.Clear();
                checkColumn.AddRange(tempObjects);
                newObjects.AddRange(tempObjects);
            }

            return (newObjects, checkColumn);
        }

        private void InstructionButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                "本转K器只支持低转高 不支持高转低\n" +
                "Key：目标key数\n" +
                "Gap：转换间隔（参数越小转换切换越快）\n" +
                "Clean：清除参数，用来清除子弹使用，例如设置为4，则会清除时长为包括1/4之内的子弹（二连音）（1/5 1/6都会清除）\n" +
                "Blank：空白列数，为0则不会生成空白列（用来减轻高k密度压力使用）\n" +
                "勾选Creator之后可更改谱面作者便于上传\n" +
                "选择对应k数之后可以转换指定k数的谱面，不勾选则默认转换全部k数的谱面\n" +
                "\n" +
                "请注意：未转换完成前请不要切换页面，否则需要重新转换\n");
        }
    }
}
