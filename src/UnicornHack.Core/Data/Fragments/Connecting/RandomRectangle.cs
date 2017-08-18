using UnicornHack.Generation;
using UnicornHack.Generation.Map;
using UnicornHack.Utils;

namespace UnicornHack.Data.Fragments
{
    public static class ConnectingMapFragmentData
    {
        public static readonly ConnectingMapFragment RandomRectangle = new ConnectingMapFragment
        {
            Name = "randomRectangle",
            GenerationWeight = new DefaultWeight(),
            DynamicMap = new RectangleMap {MinSize = new Dimensions {Width = 5, Height = 5}}
        };
    }
}