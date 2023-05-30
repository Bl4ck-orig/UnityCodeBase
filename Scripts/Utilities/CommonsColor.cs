using System.Linq;
using UnityEngine;

namespace Utilities
{
    public static class CommonsColor
    {
        #region Reverse Array -----------------------------------------------------------------
        public static Color[,] ToYReversedColorArray(this Texture2D _texture)
        {
            int width = _texture.width;
            int height = _texture.height;

            Color[,] color2D = new Color[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int yOut = 0, yIn = height - 1; yIn >= 0; yOut++, yIn--)
                {
                    color2D[x, yOut] = _texture.GetPixel(x, yIn);
                }
            }

            return color2D;
        }

        public static Color[,] ToReversedColorArray(this Texture2D _texture)
        {
            int width = _texture.width;
            int height = _texture.height;

            Color[,] color2D = new Color[width, height];
            for (int xOut = 0, xIn = width - 1; xOut < width; xOut++, xIn--)
            {
                for (int yOut = 0, yIn = height - 1; yIn >= 0; yOut++, yIn--)
                {
                    color2D[xOut, yOut] = _texture.GetPixel(xIn, yIn);
                }
            }

            return color2D;
        }


        #endregion -----------------------------------------------------------------

        #region Matrix / Texture Conversion -----------------------------------------------------------------
        public static Texture2D ToTextureApplied(this Color[,] _colorAr,
            FilterMode _filterMode = FilterMode.Point, TextureWrapMode _wrapMode = TextureWrapMode.Clamp)
        {
            int width = _colorAr.GetLength(0);
            int height = _colorAr.GetLength(1);

            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = _filterMode;
            texture.wrapMode = _wrapMode;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, _colorAr[x, y]);
                }
            }
            texture.Apply();
            return texture;
        }

        public static Texture2D ToReveresedTextureApplied(this Color[,] _colorAr, FilterMode _filterMode, TextureWrapMode _wrapMode)
        {
            int width = _colorAr.GetLength(0);
            int height = _colorAr.GetLength(1);

            Texture2D texture = new Texture2D(width, height);
            texture.filterMode = _filterMode;
            texture.wrapMode = _wrapMode;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, height - y - 1, _colorAr[x, y]);
                }
            }
            texture.Apply();
            return texture;
        }

        public static Texture2D ToRawTexture(this bool[,] _bitAr)
        {
            int width = _bitAr.GetLength(0);
            int height = _bitAr.GetLength(1);

            Texture2D texture = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, (_bitAr[x, y]) ? Color.white : Color.black);
                }
            }
            return texture;
        }

        public static Texture2D ToRawTexture(this int[,] _grayscaleAr, int _maxValue)
        {
            int width = _grayscaleAr.GetLength(0);
            int height = _grayscaleAr.GetLength(1);

            Texture2D texture = new Texture2D(width, height);
            Color color;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float colorValue = Mathf.InverseLerp(0, _maxValue, Mathf.Clamp(_grayscaleAr[x, y], 0, _maxValue));
                    color = new Color(colorValue, colorValue, colorValue, 1f);
                    texture.SetPixel(x, y, color);
                }
            }
            return texture;
        }

        public static Texture2D ToRawTexture(this int[,] _grayscaleAr, int _maxValue, Color _zeroColor)
        {
            int width = _grayscaleAr.GetLength(0);
            int height = _grayscaleAr.GetLength(1);

            Texture2D texture = new Texture2D(width, height);
            Color color;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    float colorValue = Mathf.InverseLerp(0, _maxValue, Mathf.Clamp(_grayscaleAr[x, y], 0, _maxValue));
                    color = (colorValue == 0) ? _zeroColor : new Color(colorValue, colorValue, colorValue, 1f);
                    if (_grayscaleAr[x, y] < 0)
                        color = Color.red;
                    texture.SetPixel(x, y, color);
                }
            }
            return texture;
        }

        public static Texture2D ToRawTexture(this float[,] _grayscaleAr)
        {
            int width = _grayscaleAr.GetLength(0);
            int height = _grayscaleAr.GetLength(1);

            Texture2D texture = new Texture2D(width, height);
            Color color;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    color = new Color(_grayscaleAr[x, y], _grayscaleAr[x, y], _grayscaleAr[x, y], _grayscaleAr[x, y]);
                    texture.SetPixel(x, y, color);
                }
            }
            return texture;
        }

        public static Texture2D ToRawTexture(this Color[,] _colorAr)
        {
            int width = _colorAr.GetLength(0);
            int height = _colorAr.GetLength(1);

            Texture2D texture = new Texture2D(width, height);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    texture.SetPixel(x, y, _colorAr[x, y]);
                }
            }
            return texture;
        }

        public static Color[,] To2DColorArray(this Texture2D _texture)
        {
            Color[] color1D = _texture.GetPixels();

            int width = _texture.width;
            int height = _texture.height;

            Color[,] color2D = new Color[width, height];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    color2D[x, y] = color1D[x + y * width];
                }
            }

            return color2D;
        }
        #endregion -----------------------------------------------------------------

        #region Masking -----------------------------------------------------------------

        public static Color[,] MergeMask(Color[,] _matrix, Color[,] _into, Color _maskColor)
        {
            int width = _matrix.GetLength(0);
            int height = _matrix.GetLength(1);
            Color[,] mergedMatrix = _into.Clone() as Color[,];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    mergedMatrix[x, y] = (_into[x, y] == _maskColor) ? _matrix[x, y] : _into[x, y];
                }
            }

            return mergedMatrix;
        }

        public static Color[,] CreateMask(Color[,] _matrix, Color _maskColor, int _xFrom, int _xTo, int _yFrom, int _yTo)
        {
            int width = _matrix.GetLength(0);
            int height = _matrix.GetLength(1);
            Color[,] mask = new Color[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x.IsInRange(_xFrom, _xTo) &&
                        y.IsInRange(_yFrom, _yTo))
                        mask[x, y] = _maskColor;
                    else
                        mask[x, y] = _matrix[x, y];
                }
            }

            return mask;
        }

        public static float GetAngle(Vector3 _from, Vector3 _to) => Vector3.Angle(_from, _to);

        public static float GetSignedAngle(Vector3 _from, Vector3 _to) => Vector3.SignedAngle(_from, _to, Vector3.up);

        public static Color[,] CreateMask(Color[,] _matrix, Color _maskColor, params ColorDistances[] _includedColorDistances)
        {
            int width = _matrix.GetLength(0);
            int height = _matrix.GetLength(1);
            Color[,] mask = new Color[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color col = _matrix[x, y];

                    if (_includedColorDistances.Select(x => x.Color).Contains(col))
                    {
                        mask[x, y] = col;
                        continue;
                    }

                    bool maskedColorNearby = false;
                    for (int i = 0; i < _includedColorDistances.Length; i++)
                    {
                        if (IsColorNearby(_matrix, x, y, _includedColorDistances[i].Distance, _includedColorDistances[i].Color))
                        {
                            mask[x, y] = Color.clear;
                            maskedColorNearby = true;
                            break;
                        }
                    }

                    if (!maskedColorNearby)
                        mask[x, y] = _maskColor;
                }
            }

            return mask;
        }

        public static Color[,] CreateReverseMask(Color[,] _matrix, Color _maskColor, params ColorDistances[] _excludedColorDistances)
        {
            int width = _matrix.GetLength(0);
            int height = _matrix.GetLength(1);
            Color[,] mask = new Color[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color col = _matrix[x, y];

                    if (_excludedColorDistances.Select(x => x.Color).Contains(col))
                    {
                        mask[x, y] = _maskColor;
                        continue;
                    }

                    bool maskedColorNearby = false;
                    for (int i = 0; i < _excludedColorDistances.Length; i++)
                    {
                        if (IsColorNearby(_matrix, x, y, _excludedColorDistances[i].Distance, _excludedColorDistances[i].Color))
                        {
                            mask[x, y] = _maskColor;
                            maskedColorNearby = true;
                            break;
                        }
                    }

                    if (!maskedColorNearby)
                        mask[x, y] = col;
                }
            }

            return mask;
        }

        private static bool IsColorNearby(Color[,] _matrix, int _x, int _y, int _distanceToColor, params Color[] _includeColors)
        {
            int width = _matrix.GetLength(0);
            int height = _matrix.GetLength(1);
            for (int x = Mathf.Max(0, _x - _distanceToColor); x <= Mathf.Min(width - 1, _x + _distanceToColor); x++)
            {
                for (int y = Mathf.Max(0, _y - _distanceToColor); y <= Mathf.Min(height - 1, _y + _distanceToColor); y++)
                {
                    if (_includeColors.Contains(_matrix[x, y]))
                        return true;
                }
            }
            return false;
        }

        public static Color[,] ReplaceMaskEdgeColors(Color[,] _matrix, Color _colorToInsert, Color _maskColor)
        {
            Color[,] copy = _matrix.Clone() as Color[,];

            int width = _matrix.GetLength(0);
            int height = _matrix.GetLength(1);
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_matrix[x, y] == _maskColor)
                        continue;

                    if (!IsTileAtMaskEdge(x, y, width, height, _matrix, _maskColor))
                        continue;

                    copy[x, y] = _colorToInsert;
                }
            }

            return copy;
        }

        private static bool IsTileAtMaskEdge(int _x, int _y, int _width, int _height, Color[,] _matrix, Color _maskColor)
        {
            if (_x - 1 >= 0 && _matrix[_x - 1, _y] == _maskColor)
                return true;
            if (_x + 1 < _width && _matrix[_x + 1, _y] == _maskColor)
                return true;
            if (_y - 1 >= 0 && _matrix[_x, _y - 1] == _maskColor)
                return true;
            if (_y + 1 < _height && _matrix[_x, _y + 1] == _maskColor)
                return true;
            return false;
        }
        #endregion -----------------------------------------------------------------

        #region Single Color Methods -----------------------------------------------------------------
        public static Color SetAlpha(this Color _color, float _alpha) => new Color(_color.r, _color.g, _color.b, _alpha);

        public static bool IsColorEqual(Color32 _fst, Color32 _snd) =>
            _fst.r == _snd.r && _fst.g == _snd.g && _fst.b == _snd.b && _fst.a == _snd.a;

        public static float Distance(this Color _input, Color _other) =>
            (Mathf.Abs(_input.r - _other.r) + Mathf.Abs(_input.g - _other.g) + Mathf.Abs(_input.b - _other.b)) / 3;

        public static string ToHex(this Color32 _color) =>
            "#" + _color.r.ToString("X2") + _color.g.ToString("X2") + _color.b.ToString("X2");
        #endregion -----------------------------------------------------------------
        
        #region Color Matrix Editing -----------------------------------------------------------------
        public static Color[,] ClearAlphaColors(this Color[,] _array)
        {
            int width = _array.GetLength(0);
            int height = _array.GetLength(1);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (_array[x, y].a == 0)
                        _array[x, y] = Color.clear;
                }
            }

            return _array;
        }

        public static Color[,] ReplaceColor(Color[,] _matrix, Color _colorToInsert, params Color[] _colorsToReplace)
        {
            int width = _matrix.GetLength(0);
            int height = _matrix.GetLength(1);
            Color[,] lookup = _matrix.Clone() as Color[,];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color col = _matrix[x, y];
                    if (_colorsToReplace.Contains(col))
                        lookup[x, y] = _colorToInsert;
                }
            }

            return lookup;
        }
        #endregion -----------------------------------------------------------------

        #region Color Serialization -----------------------------------------------------------------
        public static Color[,] SerializableColorsToColorMatrix(SerializableColor[,] _serializableColors)
        {
            int width = _serializableColors.GetLength(0);
            int height = _serializableColors.GetLength(1);

            Color[,] deserializedMatrix = new Color[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    deserializedMatrix[x, y] = _serializableColors[x, y].ToColor();
                }
            }

            return deserializedMatrix;
        }

        public static SerializableColor[,] CreateSerializedColorMatrix(Color[,] _colorMatrix)
        {
            int width = _colorMatrix.GetLength(0);
            int height = _colorMatrix.GetLength(1);

            SerializableColor[,] serializedMatrix = new SerializableColor[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    serializedMatrix[x, y] = new SerializableColor(_colorMatrix[x, y]);
                }
            }

            return serializedMatrix;
        }
        #endregion -----------------------------------------------------------------

        /// <summary>
        /// Generates a noise map by using a perlin noise algorithm.
        /// </summary>
        /// <param name="_mapWidth">Width of the map</param>
        /// <param name="_mapHeight">Height of the map</param>
        /// <param name="_seed">Seed of the noise</param>
        /// <param name="_scale">Scale of the map</param>
        /// <param name="_octaves">Octaves for the perlin noise</param>
        /// <param name="_persistance">Persistance of the perlin noise</param>
        /// <param name="_lacunarity">Lacunarity of the perlin noise</param>
        /// <param name="_offset">Offset for the noise map</param>
        /// <param name="_useChunkSystem">If chunks are used</param>
        /// <param name="_averageHeightModifier">Modifier for the height</param>
        /// <returns>2D float array containing noise map</returns>
        public static float[,] GenerateNoiseMap(int _mapWidth, int _mapHeight, int _seed, float _scale, int _octaves, float _persistance,
            float _lacunarity, Vector2 _offset, bool _useChunkSystem, float _averageHeightModifier)
        {
            float[,] noiseMap = new float[_mapWidth, _mapHeight];

            System.Random prng = new System.Random(_seed);
            Vector2[] octaveOffsets = new Vector2[_octaves];

            float maxPossibleHeight = 0;
            float amplitude = 1;
            float frequency = 1;

            // Set up octaves.
            for (int i = 0; i < _octaves; i++)
            {
                float offsetX = prng.Next(-100000, 100000) + _offset.x;
                float offsetY = prng.Next(-100000, 100000) - _offset.y;
                octaveOffsets[i] = new Vector2(offsetX, offsetY);

                maxPossibleHeight += amplitude;
                amplitude *= _persistance;
            }

            // Clamp scale.
            if (_scale <= 0)
                _scale = 0.0001f;

            float maxLocalNoiseHeight = float.MinValue;
            float minLocalNoiseHeight = float.MaxValue;

            float halfWidth = _mapWidth / 2f;
            float halfHeight = _mapHeight / 2f;

            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    amplitude = 1;
                    frequency = 1;
                    float noiseHeight = 0;

                    // Iterate over all octaves ...
                    for (int i = 0; i < _octaves; i++)
                    {
                        float sampleX = (x - halfWidth + octaveOffsets[i].x) / _scale * frequency;
                        float sampleY = (y - halfHeight + octaveOffsets[i].y) / _scale * frequency;

                        // ... and apply height by using perlinnoise ...
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                        // ... the amplitude, persistance and lacunarity.
                        noiseHeight += perlinValue * amplitude;

                        // ... and persitance (how much the amplitude decreases over increasing octaves )
                        amplitude *= _persistance;

                        // ... and lacunarity (how frequent the octaves increase)
                        frequency *= _lacunarity;
                    }

                    // Clamp the values.
                    if (noiseHeight > maxLocalNoiseHeight)
                        maxLocalNoiseHeight = noiseHeight;
                    else if (noiseHeight < minLocalNoiseHeight)
                        minLocalNoiseHeight = noiseHeight;

                    noiseMap[x, y] = noiseHeight;
                }
            }

            for (int y = 0; y < _mapHeight; y++)
            {
                for (int x = 0; x < _mapWidth; x++)
                {
                    // Unify height.
                    if (!_useChunkSystem)
                        noiseMap[x, y] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap[x, y]);
                    else
                    {
                        float normalizedHeight = (noiseMap[x, y] + 1) / (maxPossibleHeight / _averageHeightModifier);
                        noiseMap[x, y] = Mathf.Clamp(normalizedHeight, 0, int.MaxValue);
                    }
                }
            }

            return noiseMap;
        }

        
    }
}
