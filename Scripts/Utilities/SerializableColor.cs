using System;
using UnityEngine;

namespace Utilities
{
    [Serializable]
    public class SerializableColor
    {
        public float Red;
        public float Green;
        public float Blue;
        public float Alpha;

        public SerializableColor(Color _color)
        {
            Red = _color.r;
            Green = _color.g;
            Blue = _color.b;
            Alpha = _color.a;
        }

        public Color ToColor()
        {
            return new Color(Red, Green, Blue, Alpha);
        }
    }
}
