using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using DestinyMap.Converters;

namespace DestinyMap.Hex
{
    public class HexTile : IEquatable<HexTile>
    {
        private const string BaseUrl = "https://tjl.co/corridors-of-time/viewer.html#";

        public int Id { get; set; }
        public Symbol Symbol { get; set; }
        public bool[] Walls { get; set; }
        public SmallHexGroup[] Nodes { get; set; } = new SmallHexGroup[6];
        public string Status { get; set; }
        public string Url { get; set; }
        public string ImageUrl { get; set; }

        // Dupe checking
        public int DupeWith { get; set; }
        public bool ImageDupe { get; set; }
        public bool NodeDupe { get; set; }
        public bool ForceDelete { get; set; }

        public SmallHexGroup Top => Nodes[0];
        public SmallHexGroup TopRight => Nodes[1];
        public SmallHexGroup BottomRight => Nodes[2];
        public SmallHexGroup Bottom => Nodes[3];
        public SmallHexGroup BottomLeft => Nodes[4];
        public SmallHexGroup TopLeft => Nodes[5];

        public bool Equals(HexTile other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Id == other.Id && Symbol == other.Symbol && Equals(Walls, other.Walls) && Equals(Nodes, other.Nodes) && Url == other.Url;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((HexTile)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Id;
                hashCode = (hashCode * 397) ^ (int)Symbol;
                hashCode = (hashCode * 397) ^ (Walls != null ? Walls.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Nodes != null ? Nodes.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (Url != null ? Url.GetHashCode() : 0);
                return hashCode;
            }
        }

        public static HexTile ImportRow(CsvReader csv, int id)
        {
            try
            {
                HexTile tile = new HexTile
                {
                    Id = id,
                    Symbol = SymbolConverter.Get(csv.GetField(1)[0].ToString()),
                    ImageUrl = csv.GetField(0)
                };

                int offset = 1; //id >= 2366 ? 1 : 0;

                tile.ForceDelete = false; //id >= 2366;

                // Import the wall.
                string wall = csv.GetField(2);
                tile.Walls = new bool[6];
                tile.Walls[0] = !wall.Contains((1 - offset).ToString());
                tile.Walls[1] = !wall.Contains((2 - offset).ToString());
                tile.Walls[2] = !wall.Contains((3 - offset).ToString());
                tile.Walls[3] = !wall.Contains((4 - offset).ToString());
                tile.Walls[4] = !wall.Contains((5 - offset).ToString());
                tile.Walls[5] = !wall.Contains((6 - offset).ToString());

                for (int i = 0; i < 6; i++)
                {
                    tile.Nodes[i] = SmallHexGroup.Import(csv, i);
                }

                tile.Status = csv.GetField(9);

                // Populate the URL for checking.
                string data = "";

                for (int i = 0; i < 9; i++)
                {
                    data += $"{csv.GetField(i)}\t";
                }

                tile.Url = BaseUrl + Convert.ToBase64String(Encoding.ASCII.GetBytes(data));

                return tile;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public class HexTileForMerge
    {
        public string ImageUrl { get; set; }
        public string Center { get; set; }
        public string Holes { get; set; }
        public string Link0 => Links[0];
        public string Link1 => Links[1];
        public string Link2 => Links[2];
        public string Link3 => Links[3];
        public string Link4 => Links[4];
        public string Link5 => Links[5];
        private string[] Links { get; } = new string[6];

        public HexTileForMerge(HexTile tile)
        {
            ImageUrl = tile.ImageUrl;
            Center = tile.Symbol.ToString();

            List<int> nums = new List<int>();

            for (int i = 0; i < 6; i++)
            {
                if (tile.Walls[i])
                {
                    continue;
                }

                nums.Add(i);
            }

            Holes = string.Join(",", nums);

            for (int i = 0; i < 6; i++)
            {
                Links[i] = string.Join("", tile.Nodes[i].HexTiles.Select(t => SymbolConverter.Get(t.Symbol)));
            }
        }
    }

    public class HexTileMap : ClassMap<HexTileForMerge>
    {
        public HexTileMap()
        {
            Map(m => m.ImageUrl).Index(0);
            Map(m => m.Center).Index(1);
            Map(m => m.Holes).Index(2);
            Map(m => m.Link0).Index(3);
            Map(m => m.Link1).Index(4);
            Map(m => m.Link2).Index(5);
            Map(m => m.Link3).Index(6);
            Map(m => m.Link4).Index(7);
            Map(m => m.Link5).Index(8);
        }
    }
}
