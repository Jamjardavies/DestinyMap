using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CsvHelper;
using DestinyMap.Hex;

namespace DestinyMap
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string FileToRead = @"C:\Users\Jamjar\Downloads\CombinedStreamer-Vault Verify - Primary.csv";
        //private const string FileToRead = @"C:\Users\Jamjar\Downloads\MergeData.csv";
        
        List<string> invalidStatus = new List<string> { "Invalid", "Duplicate", "Bad Image", "Tool Only" };

        private List<HexTile> m_tiles = new List<HexTile>();
        private StringBuilder m_sb = new StringBuilder();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Update_Clicked(object sender, RoutedEventArgs e)
        {
            if (!File.Exists(txtPath.Text))
            {
                Output.Text = "Invalid Path!";
                return;
            }
            
            m_sb.Clear();
            Output.Text = "";

            string path = txtPath.Text;

            await Task.Run(() => Run(path));

            Output.Text = m_sb.ToString();
        }

        private void Run(string path)
        {
            int index = 1;
            bool checkSix = false;

            using (TextReader reader = new StreamReader(path))
            {
                using (CsvReader csv = new CsvReader(reader, new System.Globalization.CultureInfo("en_US")))
                {
                    csv.Read();

                    while (csv.Read())
                    {
                        index++;

                        if (checkSix && csv.GetField(2).Contains("6") && !invalidStatus.Contains(csv.GetField(9)))
                        {
                            m_sb.AppendLine($"Row {index} contains a 6!");
                        }

                        m_tiles.Add(HexTile.ImportRow(csv, index));
                    }
                }
            }

            m_tiles.RemoveAll(t => t == null);
            Cleaner(m_tiles);

            DuplicateRemover(m_tiles);
            Process(m_tiles);

            Console.WriteLine("Imported");
        }

        private void Cleaner(List<HexTile> tiles)
        {
            IEnumerable<HexTile> tilesToRemove = tiles.Where(t => t.Id > 20 && invalidStatus.Contains(t.Status) || t.Nodes.Any(n => !n.IsValid)).ToArray();
            IEnumerable<HexTile> skipMessage = tiles.Where(t => invalidStatus.Contains(t.Status)).ToList();

            foreach (HexTile tile in tilesToRemove)
            {
                if (!skipMessage.Contains(tile))
                {
                    m_sb.AppendLine($"Invalid data on {tile.Id}, removing");
                }

                tiles.Remove(tile);
            }
        }

        private void DuplicateRemover(List<HexTile> tiles)
        {
            // Parse the data, check for dupes.
            tiles.ForEach(test => tiles.ForEach(t => Check(t, test)));

            // Delete any force deletes.
            tiles.RemoveAll(t => t.ForceDelete);

            // Now remove the dupes.
            IEnumerable<HexTile> dupes = tiles.Where(t => t.DupeWith > 0).ToList();
            IEnumerable<HexTile> nonDupes = tiles.Where(t => t.DupeWith <= 0).ToList();

            m_sb.AppendLine($"Total Possible Clean Data: {nonDupes.Count()}");
            m_sb.AppendLine($"Total Possible Duplicates: {dupes.Count()}");

            foreach (HexTile dupe in dupes)
            {
                m_sb.AppendLine($"Possible duplicate on {dupe.Id.ToString().PadRight(5)} Looks like {dupe.DupeWith.ToString().PadRight(5)} Reason: {(dupe.ImageDupe && dupe.NodeDupe ? "Image & Node" : dupe.ImageDupe ? "Image" : "Node")}.");
                tiles.Remove(dupe);
            }

            //using (TextWriter writer = new StreamWriter("Merge.csv"))
            //{
            //    CsvHelper.Configuration.CsvConfiguration conf = new CsvHelper.Configuration.CsvConfiguration(new System.Globalization.CultureInfo("en-US"));
            //    conf.RegisterClassMap<HexTileMap>();

            //    using (CsvWriter csv = new CsvWriter(writer, conf))
            //    {
            //        foreach (HexTile tile in nonDupes)
            //        {
            //            csv.WriteRecord(new HexTileForMerge(tile));
            //            csv.NextRecord();
            //        }
            //    }
            //}
        }

        private void Check(HexTile t, HexTile test)
        {
            int match = 0;

            // Skip checking already dupes with others.
            if (test.DupeWith > 0)
            {
                return;
            }

            if (!t.Equals(test) && t.ImageUrl == test.ImageUrl)
            {
                if (t.Id < test.Id)
                {
                    t.DupeWith = test.Id;
                    t.ImageDupe = true;
                }
                else
                {
                    test.DupeWith = t.Id;
                    test.ImageDupe = true;
                }
            }

            for (int i = 0; i < 6; i++)
            {
                if (t.Equals(test) || test.Nodes[i].HexTiles.All(s => s.Symbol == Symbol.Blank))
                {
                    continue;
                }

                for (int j = 0; j < 6; j++)
                {
                    if (t.Nodes[j].HexTiles.SequenceEqual(test.Nodes[i].HexTiles))
                    {
                        match++;
                    }
                }
            }

            if (match > 1)
            {
                if (t.Id < test.Id)
                {
                    t.DupeWith = test.Id;
                    t.NodeDupe = true;
                }
                else
                {
                    test.DupeWith = t.Id;
                    test.NodeDupe = true;
                }
            }
        }

        private void Process(List<HexTile> tiles)
        {
        }
    }
}
