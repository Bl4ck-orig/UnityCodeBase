using UnityEngine;

namespace Utilities
{
    public struct ColorDistances
    {
        public Color Color;
        public int Distance;

        public ColorDistances(Color color, int distance)
        {
            Color = color;
            Distance = distance;
        }
    }
}
