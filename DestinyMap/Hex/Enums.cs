using Newtonsoft.Json;

namespace DestinyMap.Hex
{
    //[JsonConverter(typeof(JsonSymbolConverter))]
    public enum Symbol
    {
        Blank,
        Plus,
        Hex,
        Clover,
        Snake,
        Diamond,
        Cauldron
    }
}
