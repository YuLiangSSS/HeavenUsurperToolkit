﻿using Microsoft.Win32;
using System.IO;
using System.Windows;

namespace HeavenUsurperToolkit
{
    public class OsuFileProcessor
    { 
        /// <summary>
        /// Return null if osu file format is not v14 or not mania mode.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static OsuFileV14 ReadFile(FileInfo file)
        {
            List<ManiaHitObject> obj = new List<ManiaHitObject>();
            List<TimingPoint> points = new List<TimingPoint>();
            List<Colour> colours = new List<Colour>();
            StreamReader reader = new StreamReader(file.FullName);
            Metadata meta = new Metadata();
            General general = new General();
            Event events = new Event();

            bool Events = false;
            bool TimingPoints = false;
            bool Colours = false;
            bool HitObjects = false;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();
                if (line == string.Empty)
                {
                    continue;
                }

                if (line.StartsWith("osu file format"))
                {
                    //general.Version = line.Split(' ').Last();
                    continue;
                }

                // General

                if (line.StartsWith("AudioFilename"))
                {
                    general.AudioFilename = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("AudioLeadIn"))
                {
                    general.AudioLeadIn = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("PreviewTime"))
                {
                    general.PreviewTime = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Countdown"))
                {
                    general.Countdown = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("SampleSet"))
                {
                    general.SampleSet = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("StackLeniency"))
                {
                    general.StackLeniency = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Mode"))
                {
                    general.Mode = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("LetterboxInBreaks"))
                {
                    general.LetterboxInBreaks = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("SpecialStyle"))
                {
                    general.SpecialStyle = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("WidescreenStoryboard"))
                {
                    general.WidescreenStoryboard = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }

                // Metadata

                if (line.StartsWith("TitleUnicode"))
                {
                    meta.TitleUnicode = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("ArtistUnicode"))
                {
                    meta.ArtistUnicode = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Title"))
                {
                    meta.Title = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Artist"))
                {
                    meta.Artist = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Creator"))
                {
                    meta.Creator = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Version"))
                {
                    meta.Difficulty = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Source"))
                {
                    meta.Source = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Tags"))
                {
                    meta.Tags = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("BeatmapID"))
                {
                    meta.BeatmapID = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("BeatmapSetID"))
                {
                    meta.BeatmapSetID = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }

                // Difficulty

                if (line.StartsWith("HPDrainRate"))
                {
                    general.HPDrainRate = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }
                if (line.StartsWith("CircleSize"))
                {
                    general.CircleSize = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }
                if (line.StartsWith("OverallDifficulty"))
                {
                    general.OverallDifficulty = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }
                if (line.StartsWith("ApproachRate"))
                {
                    general.ApproachRate = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }
                if (line.StartsWith("SliderMultiplier"))
                {
                    general.SliderMultiplier = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }
                if (line.StartsWith("SliderTickRate"))
                {
                    general.SliderTickRate = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }

                // [HitObjects] & [TimingPoints] & [Colours] & [Events]

                if (line.StartsWith("[Events]"))
                {
                    Events = true;
                    continue;
                }

                if (line.StartsWith("[TimingPoints]"))
                {
                    Events = false;
                    TimingPoints = true;
                    continue;
                }

                if (line.StartsWith("[Colours]"))
                {
                    Events = false;
                    TimingPoints = false;
                    Colours = true;
                    continue;
                }

                if (line.StartsWith("[HitObjects]"))
                {
                    Events = false;
                    TimingPoints = false;
                    Colours = false;
                    HitObjects = true;
                    continue;
                }

                if (Events)
                {
                    events.eventString += line + Environment.NewLine;
                    continue;
                }

                if (TimingPoints)
                {
                    var pointElements = line.Split(',');
                    points.Add(new TimingPoint((int)double.Parse(pointElements[0]), double.Parse(pointElements[1]), (int)double.Parse(pointElements[2]), (int)double.Parse(pointElements[3]), (int)double.Parse(pointElements[4]), (int)double.Parse(pointElements[5]), (int)double.Parse(pointElements[6]), (int)double.Parse(pointElements[7])));
                    continue;
                }

                if (Colours)
                {
                    var colourElements = line.Split(':');
                    var name = colourElements[0].Trim();
                    var colour = colourElements[1].Trim().Split(',');
                    colours.Add(new Colour(name, (byte)double.Parse(colour[0]), (byte)double.Parse(colour[1]), (byte)double.Parse(colour[2])));
                    continue;
                }

                if (!HitObjects)
                {
                    continue;
                }
                var elements = line.Split(',');
                var x = (int)double.Parse(elements[0]);
                var y = (int)double.Parse(elements[1]);
                var time = (int)double.Parse(elements[2]);
                var type = (int)double.Parse(elements[3]);
                var hitSound = (int)double.Parse(elements[4]);
                //var objectParams = elements[4];
                var hitSample = elements[5];
                obj.Add(new ManiaHitObject(x, y, (int)general.CircleSize, time, type, hitSound, hitSample));
            }
            reader.Close();
            return new OsuFileV14(obj, points, colours, meta, general, events, file);
        }

        public static OsuFileV14 ReadFile(string fileName)
        {
            return ReadFile(new FileInfo(fileName));
        }

        public static void WriteOsuFile(OsuFileV14 osu, string path = "")
        {
            if (path != "")
            {
                osu.WriteFile(path);
                return;
            }
            osu.WriteFile(osu.path);
        }

        public string GetOsuPath()
        {
            try
            {
                //string name = "osu";
                string path = @"\osu!\shell\open\command";
                string keyname = "";
                RegistryKey regkey = Registry.ClassesRoot;
                RegistryKey? regsubkey = regkey.OpenSubKey(path, false);
                RegistryValueKind regvaluekind = regsubkey.GetValueKind(keyname);
                var result = regsubkey.GetValue(keyname);
                if (regvaluekind == RegistryValueKind.String)
                {
                    var destination = result.ToString();
                    return destination.Substring(1, destination.Length - 16);// return "*\osupath";
                }
            }
            catch
            {
                MessageBox.Show("无法找到osu目录", "错误");
                return "Failed";
            }
            MessageBox.Show("无法找到osu目录", "错误");
            return "Failed";
        }
    }

    public class OsuFileV14
    {
        public List<ManiaHitObject> HitObjects = [];
        public List<TimingPoint> TimingPoints = new List<TimingPoint>();
        public List<Colour> Colours = new List<Colour>();
        public Metadata Metadata;
        public General General;
        public Event Events;
        public FileInfo OriginalFile;
        public string path = string.Empty;
        public const string FileExtension = ".osu";
        public readonly char[] ForbiddenChar = { '\\', '/', ':', '*', '?', '\"', '<', '>', '|' }; // Windows' forbidden char in file.
        public double StarRating = 0;

        public static readonly Dictionary<int, List<int>> KeyX = new Dictionary<int, List<int>>
        {
            { 3, [85, 256, 426] },
            { 4, [64, 192, 320, 448] },
            { 5, [51, 153, 256, 358, 460] },
            { 6, [42, 128, 213, 298, 384, 469] },
            { 7, [36, 109, 182, 256, 329, 402, 475] },
            { 8, [32, 96, 160, 224, 288, 352, 416, 480] },
            { 9, [28, 85, 142, 199, 256, 312, 369, 426, 483] },
            { 10, [25, 76, 128, 179, 230, 281, 332, 384, 435, 486] },
            { 12, [] },
            { 14, [] },
            { 16, [] },
            { 18, [] },
        };

        public string FileName
        {
            get
            {
                return string.Concat(OriginalName.Where(c => !ForbiddenChar.Contains(c)));
            }
        } // Lazer: Put "_" to replace forbidden char or some other char like (').

        public string OriginalName
        {
            get
            {
                return Metadata.Artist + " - " + Metadata.Title + " (" + Metadata.Creator + ") " + "[" + Metadata.Difficulty + "]";
            }
        }

        public OsuFileV14(List<ManiaHitObject> hitObjects, List<TimingPoint> timingPoints, List<Colour> colours, Metadata metadata, General general, Event events, FileInfo file)
        {
            this.HitObjects = hitObjects;
            this.TimingPoints = timingPoints;
            this.Colours = colours;
            this.Metadata = metadata;
            this.General = general;
            this.Events = events;
            this.OriginalFile = file;
            this.path = file.DirectoryName ?? string.Empty;
            for (int i = 0; i < hitObjects.Count; i++)
            {
                var obj = hitObjects[i];
                obj.CircleSize = (int)general.CircleSize;
                obj.actualColumn = (int)Math.Floor(obj.X * obj.CircleSize / 512.0);
                hitObjects[i] = obj;
            }

            try
            {
                StarRating = StarRatingCalculator.CalculateStarRating(hitObjects, General.OverallDifficulty, (int)General.CircleSize, 1.0);
            }
            catch (Exception ex)
            {
#if DEBUG
                Task.Run(() =>
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
                });
#endif
                StarRating = 0;
            }
        }

        public OsuFileV14(string FileName)
        {
            var osu = OsuFileProcessor.ReadFile(FileName);
            this.HitObjects = osu.HitObjects;
            this.TimingPoints = osu.TimingPoints;
            this.Metadata = osu.Metadata;
            this.General = osu.General;
            this.path = Path.GetDirectoryName(FileName) ?? string.Empty;
            this.OriginalFile = new FileInfo(FileName);
            for (int i = 0; i < HitObjects.Count; i++)
            {
                var obj = HitObjects[i];
                obj.CircleSize = (int)osu.General.CircleSize;
                obj.actualColumn = (int)Math.Floor(obj.X * obj.CircleSize / 512.0);
                HitObjects[i] = obj;
            }

            try
            {
                StarRating = StarRatingCalculator.CalculateStarRating(HitObjects, General.OverallDifficulty, (int)General.CircleSize, 1.0);
            }
            catch (Exception ex)
            {
#if DEBUG
                Task.Run(() =>
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.None, MessageBoxOptions.DefaultDesktopOnly);
                });
#endif
                StarRating = 0;
            }
        }

        public TimingPoint TimingPointAt(double time)
        {
            if (TimingPoints.Count == 0)
            {
                throw new InvalidOperationException("TimingPoints is empty.");
            }

            var RedPoints = TimingPoints.Where(tp => tp.BeatLength >= 0).OrderBy(tp => tp.Time).ToList();

            if (RedPoints.Count == 1)
            {
                return RedPoints[0];
            }

            for (int i = 0; i < RedPoints.Count - 1; i++)
            {
                if (RedPoints[i].Time <= time && RedPoints[i + 1].Time > time)
                {
                    return RedPoints[i];
                }
            }

            return RedPoints[RedPoints.Count - 1];
        }

        public void ManiaToKeys(int Keys)
        {
            General.CircleSize = Keys;
            for (int i = 0; i < HitObjects.Count; i++)
            {
                var obj = HitObjects[i];
                obj.X = KeyX[Keys][obj.Column];
                obj.CircleSize = Keys;
                HitObjects[i] = obj;
            }
            //HitObjects.ForEach(obj => obj.X = KeyX[Keys][obj.Column]);
            //HitObjects.ForEach(obj => obj.CircleSize = Keys);
        }

        public List<ManiaHitObject> SelectColumn(int Column)
        {
            List<ManiaHitObject> list = HitObjects.Where(obj => obj.Column == Column).ToList();
            //list.SetAllCircleSize(1);
            return list;
        }

        public List<ManiaHitObject> SelectManyColumn(int[] Columns)
        {
            List<ManiaHitObject> list = new List<ManiaHitObject>();
            int i = 0;
            int ToCircleSize = Columns.Length;
            foreach (int column in Columns)
            {
                var range = SelectColumn(column);
                range.ForEach(o => o.X = KeyX[ToCircleSize][i]);
                range.ForEach(o => o.CircleSize = ToCircleSize);
                list.AddRange(range);
                i++;
            }
            return list;
        }

        public void RemoveManyColumn(int[] Columns)
        {
            int[] RemainColumns = Enumerable.Range(0, (int)General.CircleSize).Where(c => !Columns.Contains(c)).ToArray();
            HitObjects = SelectManyColumn(RemainColumns);
            General.CircleSize -= Columns.Length;
        }

        public OsuFileV14 Copy()
        {
            return new OsuFileV14
            (
                new List<ManiaHitObject>(HitObjects.Select(obj => obj)),
                new List<TimingPoint>(TimingPoints.Select(pt => pt)),
                new List<Colour>(Colours.Select(col => col)),
                Metadata,
                General,
                Events,
                OriginalFile
           );
        }

        public void ReadFile(FileInfo file)
        {
            List<ManiaHitObject> obj = new List<ManiaHitObject>();
            List<TimingPoint> points = new List<TimingPoint>();
            List<Colour> colours = new List<Colour>();
            StreamReader reader = new StreamReader(file.FullName);
            Metadata meta = new Metadata();
            General gener = new General();
            Event events = new Event();

            bool Events = false;
            bool TimingPoints = false;
            bool Colours = false;
            bool HitObjects = false;

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine().Trim();
                if (line == string.Empty)
                {
                    continue;
                }

                if (line.StartsWith("osu file format"))
                {
                    //general.Version = line.Split(' ').Last();
                    continue;
                }

                // General

                if (line.StartsWith("AudioFilename"))
                {
                    gener.AudioFilename = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("AudioLeadIn"))
                {
                    gener.AudioLeadIn = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("PreviewTime"))
                {
                    gener.PreviewTime = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Countdown"))
                {
                    gener.Countdown = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("SampleSet"))
                {
                    gener.SampleSet = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("StackLeniency"))
                {
                    gener.StackLeniency = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Mode"))
                {
                    gener.Mode = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("LetterboxInBreaks"))
                {
                    gener.LetterboxInBreaks = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("SpecialStyle"))
                {
                    gener.SpecialStyle = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("WidescreenStoryboard"))
                {
                    gener.WidescreenStoryboard = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }

                // Metadata

                if (line.StartsWith("TitleUnicode"))
                {
                    meta.TitleUnicode = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("ArtistUnicode"))
                {
                    meta.ArtistUnicode = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Title"))
                {
                    meta.Title = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Artist"))
                {
                    meta.Artist = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Creator"))
                {
                    meta.Creator = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Version"))
                {
                    meta.Difficulty = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Source"))
                {
                    meta.Source = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("Tags"))
                {
                    meta.Tags = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("BeatmapID"))
                {
                    meta.BeatmapID = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }
                if (line.StartsWith("BeatmapSetID"))
                {
                    meta.BeatmapSetID = line.Substring(line.IndexOf(':') + 1).Trim(); continue;
                }

                // Difficulty

                if (line.StartsWith("HPDrainRate"))
                {
                    gener.HPDrainRate = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }
                if (line.StartsWith("CircleSize"))
                {
                    gener.CircleSize = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }
                if (line.StartsWith("OverallDifficulty"))
                {
                    gener.OverallDifficulty = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }
                if (line.StartsWith("ApproachRate"))
                {
                    gener.ApproachRate = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }
                if (line.StartsWith("SliderMultiplier"))
                {
                    gener.SliderMultiplier = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }
                if (line.StartsWith("SliderTickRate"))
                {
                    gener.SliderTickRate = double.Parse(line.Substring(line.IndexOf(':') + 1).Trim()); continue;
                }

                // [HitObjects] & [TimingPoints] & [Colours] & [Events]

                if (line.StartsWith("[Events]"))
                {
                    Events = true;
                    continue;
                }

                if (line.StartsWith("[TimingPoints]"))
                {
                    Events = false;
                    TimingPoints = true;
                    continue;
                }

                if (line.StartsWith("[Colours]"))
                {
                    Events = false;
                    TimingPoints = false;
                    Colours = true;
                    continue;
                }

                if (line.StartsWith("[HitObjects]"))
                {
                    Events = false;
                    TimingPoints = false;
                    Colours = false;
                    HitObjects = true;
                    continue;
                }

                if (Events)
                {
                    var eventElements = line.Split(',');

                    events.eventString += line + Environment.NewLine;

                    //var eventType = eventElements[0];
                    //if (eventType == "0")
                    //{ }
                    //else if (eventType == "1" || eventType == "Video")
                    //{ }
                    //else if (eventType == "2" || eventType == "Break")
                    //{ }
                    continue;
                }

                if (TimingPoints)
                {
                    var pointElements = line.Split(',');
                    points.Add(new TimingPoint((int)double.Parse(pointElements[0]), double.Parse(pointElements[1]), (int)double.Parse(pointElements[2]), (int)double.Parse(pointElements[3]), (int)double.Parse(pointElements[4]), (int)double.Parse(pointElements[5]), (int)double.Parse(pointElements[6]), (int)double.Parse(pointElements[7])));
                    continue;
                }

                if (Colours)
                {
                    var colourElements = line.Split(':');
                    var name = colourElements[0].Trim();
                    var colour = colourElements[1].Trim().Split(',');
                    colours.Add(new Colour(name, (byte)double.Parse(colour[0]), (byte)double.Parse(colour[1]), (byte)double.Parse(colour[2])));
                    continue;
                }

                if (!HitObjects)
                {
                    continue;
                }
                var elements = line.Split(',');
                var x = (int)double.Parse(elements[0]);
                var y = (int)double.Parse(elements[1]);
                var time = (int)double.Parse(elements[2]);
                var type = (int)double.Parse(elements[3]);
                var hitSound = (int)double.Parse(elements[4]);
                //var objectParams = elements[4];
                var hitSample = elements[5];
                obj.Add(new ManiaHitObject(x, y, (int)gener.CircleSize, time, type, hitSound, hitSample));
            }
            reader.Close();
            this.HitObjects = obj;
            this.TimingPoints = points;
            Metadata = meta;
            General = gener;
            OriginalFile = file;
            path = file.DirectoryName ?? string.Empty;
        }


        public void WriteFile(string path = "")
        {
            StreamWriter writer;
            if (path != "")
            {
                writer = new StreamWriter(path + Path.DirectorySeparatorChar + FileName + FileExtension);
            }
            else
            {
                writer = new StreamWriter(this.path + Path.DirectorySeparatorChar + FileName + FileExtension);
            }

            // [General]
            writer.WriteLine("osu file format " + General.Version + Environment.NewLine);
            writer.WriteLine("[General]");
            writer.WriteLine("AudioFilename:" + General.AudioFilename);
            writer.WriteLine("AudioLeadIn:" + General.AudioLeadIn);
            writer.WriteLine("PreviewTime:" + General.PreviewTime);
            writer.WriteLine("Countdown:" + General.Countdown);
            writer.WriteLine("SampleSet:" + General.SampleSet);
            writer.WriteLine("StackLeniency:" + General.StackLeniency);
            writer.WriteLine("Mode:" + General.Mode);
            writer.WriteLine("LetterboxInBreaks:" + General.LetterboxInBreaks);
            writer.WriteLine("SpecialStyle:" + General.SpecialStyle);
            writer.WriteLine("WidescreenStoryboard:" + General.WidescreenStoryboard);
            writer.WriteLine();

            // [Metadata]
            writer.WriteLine("[Metadata]");
            writer.WriteLine("Title:" + Metadata.Title);
            writer.WriteLine("TitleUnicode:" + Metadata.TitleUnicode);
            writer.WriteLine("Artist:" + Metadata.Artist);
            writer.WriteLine("ArtistUnicode:" + Metadata.ArtistUnicode);
            writer.WriteLine("Creator:" + Metadata.Creator);
            writer.WriteLine("Version:" + Metadata.Difficulty);
            writer.WriteLine("Source:" + Metadata.Source);
            writer.WriteLine("Tags:" + Metadata.Tags);
            writer.WriteLine("BeatmapID:" + Metadata.BeatmapID);
            writer.WriteLine("BeatmapSetID:" + Metadata.BeatmapSetID);
            writer.WriteLine();

            // [Difficulty]
            writer.WriteLine("[Difficulty]");
            writer.WriteLine("HPDrainRate:" + General.HPDrainRate);
            writer.WriteLine("CircleSize:" + General.CircleSize);
            writer.WriteLine("OverallDifficulty:" + General.OverallDifficulty);
            writer.WriteLine("ApproachRate:" + General.ApproachRate);
            writer.WriteLine("SliderMultiplier:" + General.SliderMultiplier);
            writer.WriteLine("SliderTickRate:" + General.SliderTickRate);
            writer.WriteLine();

            // [Events]
            writer.WriteLine("[Events]");
            writer.WriteLine(Events.eventString);
            //writer.WriteLine("//Background and Video events");
            //writer.WriteLine("//Break Periods");
            //writer.WriteLine("//Storyboard Layer 0 (Background)");
            //writer.WriteLine("//Storyboard Layer 1 (Fail)");
            //writer.WriteLine("//Storyboard Layer 2 (Pass)");
            //writer.WriteLine("//Storyboard Layer 3 (Foreground)");
            //writer.WriteLine("//Storyboard Layer 4 (Overlay)");
            //writer.WriteLine("//Storyboard Sound Samples");
            //writer.WriteLine();

            // [TimingPoints]
            writer.WriteLine("[TimingPoints]");
            foreach (var point in TimingPoints)
            {
                writer.WriteLine(string.Join(",", point.Time, point.BeatLength, point.Meter, point.SampleSet, point.SampleIndex, point.Volume, point.Uninherited, point.Effects));
            }
            writer.WriteLine();

            // [Colours]
            if (Colours.Count > 0)
            {
                writer.WriteLine("[Colours]");
                foreach (var colour in Colours)
                {
                    writer.WriteLine(colour.Name + ": " + colour.R + "," + colour.G + "," + colour.B + "," + colour.A);
                }
            }
            writer.WriteLine();

            // [HitObjects]
            writer.WriteLine("[HitObjects]");
            foreach (var obj in HitObjects)
            {
                writer.WriteLine(string.Join(",", obj.X, obj.Y, obj.StartTime, obj.Type, obj.HitSound, obj.HitSample));
            }

            writer.Close();
        }
    }

    public struct TimingPoint
    {
        public int Time;
        public double BeatLength;
        public int Meter;
        public int SampleSet;
        public int SampleIndex;
        public int Volume;
        public int Uninherited;
        public int Effects;

        public TimingPoint(int time, double beatLength, int meter, int sampleSet, int sampleIndex, int volume, int uninherited, int effects)
        {
            Time = time;
            BeatLength = beatLength;
            Meter = meter;
            SampleSet = sampleSet;
            SampleIndex = sampleIndex;
            Volume = volume;
            Uninherited = uninherited;
            Effects = effects;
        }
    }

    public struct ManiaHitObject
    {
        public int X;
        public int Y;
        public int StartTime;
        public int Type;
        public int HitSound;
        public int CircleSize;
        public string HitSample;

        public int actualColumn;

        /// <summary>
        /// Get <see cref="StartTime"/> if can't get or have no EndTime.
        /// </summary>
        public int EndTime
        {
            get
            {
                try
                {
                    if (Type != 128)
                    {
                        return StartTime;
                    }
                    int time = int.Parse(HitSample.Split(':')[0]);
                    return time;
                }
                catch
                {
                    return StartTime;
                }
            }
            set
            {
                if (value != StartTime)
                {
                    if (Type != 128)
                    {
                        Type = 128;
                    }
                    if (HitSample.Split(':').Length == 5)
                    {
                        HitSample = value + ":" + HitSample;
                    }
                    else
                    {
                        string letter = string.Join(":", HitSample.Split(':').Skip(1));
                        HitSample = value + ":" + letter;
                    }
                }
                else
                {
                    if (Type == 128)
                    {
                        Type = 1;
                        HitSample = string.Join(":", HitSample.Split(':').Skip(1));
                    }
                }
            }
        }

        public bool IsLN => StartTime != EndTime || Type == 128;

        /// <summary>
        /// Value from 0 to CircleSize - 1
        /// </summary>
        public int Column
        {
            get
            {
                return actualColumn;
            }
            set
            {
                actualColumn = value;
                X = OsuFileV14.KeyX[CircleSize][value];
            }
            //set
            //{
            //    X = (int)((value + 0.5) * 512 / CircleSize);
            //} // Will lose precision when set value.
        }

        public ManiaHitObject(int x, int y, int circleSize, int startTime, int type, int hitSound = 0, string hitSample = "0:0:0:0:", int endTime = int.MinValue)
        {
            X = x;
            Y = y;
            StartTime = startTime;
            CircleSize = circleSize;
            actualColumn = (int)Math.Floor(x * circleSize / 512.0);
            Type = type;
            HitSound = hitSound;
            HitSample = hitSample;
            if (endTime != int.MinValue)
            {
                EndTime = endTime;
            }
        }

        // Column: 0~N-1
        public ManiaHitObject(int column, int circleSize, int startTime, int type, int hitSound = 0, string hitSample = "0:0:0:0:", int endTime = int.MinValue)
        {
            actualColumn = column;
            X = (int)((column + 0.5) * 512 / circleSize);
            Y = 192;
            StartTime = startTime;
            CircleSize = circleSize;
            Type = type;
            HitSound = hitSound;
            HitSample = hitSample;
            if (endTime != int.MinValue)
            {
                EndTime = endTime;
            }
        }

        public ManiaHitObject(int column, int circleSize, int startTime, int hitSound = 0, string hitSample = "0:0:0:0:", int endTime = int.MinValue)
        {
            actualColumn = column;
            X = (int)((column + 0.5) * 512 / circleSize);
            Y = 192;
            StartTime = startTime;
            CircleSize = circleSize;
            HitSound = hitSound;
            HitSample = hitSample;
            if (endTime != int.MinValue)
            {
                EndTime = endTime;
            }
            else
            {
                Type = 1;
                return;
            }
            if (endTime != startTime)
            {
                Type = 128;
            }
            else
            {
                Type = 1;
            }
        }

        /// <summary>
        /// Convert LN to Note.
        /// </summary>
        /// <returns></returns>
        public ManiaHitObject ToNote()
        {
            ManiaHitObject note = this;
            note.EndTime = note.StartTime;
            return note;
        }
    }

    public struct Event
    {
        public string eventString;

        public Event()
        {
            eventString = string.Empty;
        }
    }

    public struct Colour
    {
        public string Name;
        public byte R;
        public byte G;
        public byte B;
        public byte A;

        public Colour(string name, byte r, byte g, byte b)
        {
            Name = name;
            R = r;
            G = g;
            B = b;
            A = 255;
        }

        public Colour(string name, byte r, byte g, byte b, byte a)
        {
            Name = name;
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }

    public struct Metadata
    {
        public string Title;
        public string TitleUnicode;
        public string Artist;
        public string ArtistUnicode;
        public string Creator;
        public string Difficulty;
        public string Source;
        public string Tags;
        public string BeatmapID;
        public string BeatmapSetID;

        public Metadata()
        {
            Title = string.Empty;
            TitleUnicode = string.Empty;
            Artist = string.Empty;
            ArtistUnicode = string.Empty;
            Creator = string.Empty;
            Difficulty = string.Empty;
            Source = string.Empty;
            Tags = string.Empty;
            BeatmapID = "0";
            BeatmapSetID = "-1";
        }

        public Metadata(string title, string titleUnicode, string artist, string artistUnicode, string creator, string version, string source, string tags, string beatmapID, string beatmapSetID)
        {
            Title = title;
            TitleUnicode = titleUnicode;
            Artist = artist;
            ArtistUnicode = artistUnicode;
            Creator = creator;
            Difficulty = version;
            Source = source;
            Tags = tags;
            BeatmapID = beatmapID;
            BeatmapSetID = beatmapSetID;
        }
    }

    public struct General
    {
        public string Version = "v14";
        public string PreviewTime;
        public string AudioFilename;
        public string AudioLeadIn;
        public string Countdown;
        public string SampleSet;
        public string StackLeniency;
        public string Mode;
        public string LetterboxInBreaks;
        public string SpecialStyle;
        public string WidescreenStoryboard;

        // Difficulty
        public double HPDrainRate;
        public double CircleSize;
        public double OverallDifficulty;
        public double ApproachRate;
        public double SliderMultiplier;
        public double SliderTickRate;

        // Events
        public string ImageEvent = string.Empty;
        public string VideoEvent = string.Empty;

        public General()
        {
            PreviewTime = string.Empty;
            AudioFilename = string.Empty;
            AudioLeadIn = string.Empty;
            Countdown = string.Empty;
            SampleSet = string.Empty;
            StackLeniency = string.Empty;
            Mode = string.Empty;
            LetterboxInBreaks = string.Empty;
            SpecialStyle = string.Empty;
            WidescreenStoryboard = string.Empty;
            HPDrainRate = 0;
            CircleSize = 8;
            OverallDifficulty = 0;
            ApproachRate = 5;
        }

        public General(string version, string previewTime, string audioFilename, string audioLeadIn, string countdown, string sampleSet, string stackLeniency, string mode, string letterboxInBreaks, string specialStyle, string widescreenStoryboard, double hPDrainRate, double circleSize, double overallDifficulty, double approachRate, double sliderMultiplier, double sliderTickRate, string imageEvent, string videoEvent)
        {
            Version = version;
            PreviewTime = previewTime;
            AudioFilename = audioFilename;
            AudioLeadIn = audioLeadIn;
            Countdown = countdown;
            SampleSet = sampleSet;
            StackLeniency = stackLeniency;
            Mode = mode;
            LetterboxInBreaks = letterboxInBreaks;
            SpecialStyle = specialStyle;
            WidescreenStoryboard = widescreenStoryboard;
            HPDrainRate = hPDrainRate;
            CircleSize = circleSize;
            OverallDifficulty = overallDifficulty;
            ApproachRate = approachRate;
            SliderMultiplier = sliderMultiplier;
            SliderTickRate = sliderTickRate;
            ImageEvent = imageEvent;
            VideoEvent = videoEvent;
        }
    }
}
