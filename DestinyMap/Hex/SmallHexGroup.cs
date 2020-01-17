using System;
using CsvHelper;
using DestinyMap.Converters;

namespace DestinyMap.Hex
{
    public class SmallHexGroup : IEquatable<SmallHexGroup>
    {
        public SmallHexTile[] HexTiles { get; set; } = new SmallHexTile[7];
        public bool IsValid { get; set; }
        
        public bool Equals(SmallHexGroup other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Equals(HexTiles, other.HexTiles) && IsValid == other.IsValid;
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

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((SmallHexGroup)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((HexTiles != null ? HexTiles.GetHashCode() : 0) * 397) ^ IsValid.GetHashCode();
            }
        }

        public static SmallHexGroup Import(CsvReader csv, int index)
        {
            SmallHexGroup group = new SmallHexGroup();
            string node = csv.GetField(3 + index).Replace(" ", "").Replace(",", "").Replace("\r", "").Replace("\n", "");

            group.IsValid = node.Length == 7;

            if (!group.IsValid)
            {
                return group;
            }

            for (int i = 0; i < 7; i++)
            {
                group.HexTiles[i] = new SmallHexTile
                {
                    Symbol = SymbolConverter.Get(node[i].ToString())
                };
            }

            return group;
        }
    }
}
