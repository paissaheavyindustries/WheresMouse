using System.Numerics;

namespace WheresMouse
{

    internal class Maustrale
    {

        public enum StyleEnum
        {
            Boingo
        }

        public Vector2 Position { get; set; }
        public StyleEnum Style { get; set; }
        public Vector4 Color { get; set; }
        public double TTL { get; set; }
        public double TTLMax { get; set; }

    }

}
