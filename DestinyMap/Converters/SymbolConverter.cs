using System;
using DestinyMap.Hex;

namespace DestinyMap.Converters
{
    public static class SymbolConverter
    {
        public static Symbol Get(string code)
        {
            switch (code)
            {
                case "H":
                case "h":
                    return Symbol.Hex;
                case "C":
                case "c":
                    return Symbol.Clover;
                case "S":
                case "s":
                    return Symbol.Snake;
                case "D":
                case "d":
                    return Symbol.Diamond;
                case "P":
                case "p":
                    return Symbol.Plus;
                case "T":
                case "t":
                    return Symbol.Cauldron;
                default:
                    return Symbol.Blank;
            }
        }

        public static string Get(Symbol symbol)
        {
            switch (symbol)
            {
                case Symbol.Blank:
                    return "B";
                
                case Symbol.Plus:
                    return "P";

                case Symbol.Hex:
                    return "H";

                case Symbol.Clover:
                    return "C";
                
                case Symbol.Snake:
                    return "S";
                
                case Symbol.Diamond:
                    return "D";

                case Symbol.Cauldron:
                    return "T";

                default:
                    throw new ArgumentOutOfRangeException(nameof(symbol), symbol, null);
            }
        }
    }
}
