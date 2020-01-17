using System;

namespace DestinyMap.Hex
{
    public class SmallHexTile : IEquatable<SmallHexTile>
    {
        public Symbol Symbol { get; set; }

        public bool Equals(SmallHexTile other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Symbol == other.Symbol;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((SmallHexTile)obj);
        }

        public override int GetHashCode()
        {
            return (int)Symbol;
        }
    }
}
