using System;
using System.Linq;

namespace Utilities
{
    public static class CommonsMatrix
    {
        #region Borders -----------------------------------------------------------------
        public static T[,] DecreaseMatrixBorders<T>(T[,] _matrix, int _decreaseRadius)
        {
            return DecreaseMatrixBorders(_matrix, _decreaseRadius, _decreaseRadius);
        }

        public static T[,] DecreaseMatrixBorders<T>(T[,] _matrix, int _decreaseRadiusX, int _decreaseRadiusY)
        {
            int originWidth = _matrix.GetLength(0);
            int originHeight = _matrix.GetLength(1);
            if (_decreaseRadiusX * 2 >= originWidth ||
                _decreaseRadiusY * 2 >= originHeight)
                throw new ArgumentException("Matrix of size: " + originWidth + " / " + originHeight +
                    " is too small to shrink by " + _decreaseRadiusX + " / " + _decreaseRadiusY);

            T[,] shrinkedMatrix = new T[originWidth - _decreaseRadiusX * 2, originHeight - _decreaseRadiusY * 2];

            for (int x = 0; x < originWidth; x++)
            {
                for (int y = 0; y < originHeight; y++)
                {
                    if (x < _decreaseRadiusX || y < _decreaseRadiusY)
                        continue;

                    if (x >= originWidth - _decreaseRadiusX || y >= originHeight - _decreaseRadiusY)
                        continue;

                    shrinkedMatrix[x - _decreaseRadiusX, y - _decreaseRadiusY] = _matrix[x, y];
                }
            }

            return shrinkedMatrix;
        }

        public static T[,] IncreaseMatrixBorders<T>(T[,] _matrix, int _increaseRadius, T _edgeValue)
        {
            return IncreaseMatrixBorders(_matrix, _increaseRadius, _increaseRadius, _edgeValue);
        }

        public static T[,] IncreaseMatrixBorders<T>(T[,] _matrix, int _increaseRadiusX, int _increaseRadiusY, T _edgeValue)
        {
            return IncreaseMatrixBorders(_matrix, _increaseRadiusX, _increaseRadiusX, _increaseRadiusY, _increaseRadiusY, _edgeValue);
        }

        public static T[,] IncreaseMatrixBorders<T>(T[,] _matrix,
            int _increaseRadiusXMinus, int _increaseRadiusXPlus, int _increaseRadiusYMinus, int _increaseRadiusYPlus, T _edgeValue)
        {
            int width = _matrix.GetLength(0) + _increaseRadiusXMinus + _increaseRadiusXPlus;
            int height = _matrix.GetLength(1) + _increaseRadiusYMinus + _increaseRadiusYPlus;
            T[,] oversizedMatrix = new T[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x < _increaseRadiusXMinus || y < _increaseRadiusYMinus)
                        oversizedMatrix[x, y] = _edgeValue;
                    else if (x >= width - _increaseRadiusXPlus || y >= height - _increaseRadiusYPlus)
                        oversizedMatrix[x, y] = _edgeValue;
                    else
                        oversizedMatrix[x, y] = _matrix[x - _increaseRadiusXMinus, y - _increaseRadiusYMinus];
                }
            }

            return oversizedMatrix;
        }
        #endregion -----------------------------------------------------------------


        #region Arithmetics -----------------------------------------------------------------
        public static float[,] DotProduct(this float[,] _matrix1, float[,] _matrix2)
        {
            var matrix1Rows = _matrix1.GetLength(0);
            var matrix1Cols = _matrix1.GetLength(1);
            var matrix2Rows = _matrix2.GetLength(0);
            var matrix2Cols = _matrix2.GetLength(1);

            if (matrix1Cols != matrix2Rows)
                throw new InvalidOperationException
                  ("Product is undefined. n columns of first matrix must equal to n rows of second matrix");

            float[,] product = new float[matrix1Rows, matrix2Cols];

            for (int matrix1_row = 0; matrix1_row < matrix1Rows; matrix1_row++)
            {
                for (int matrix2_col = 0; matrix2_col < matrix2Cols; matrix2_col++)
                {
                    for (int matrix1_col = 0; matrix1_col < matrix1Cols; matrix1_col++)
                    {
                        product[matrix1_row, matrix2_col] +=
                          _matrix1[matrix1_row, matrix1_col] *
                          _matrix2[matrix1_col, matrix2_col];
                    }
                }
            }

            return product;
        }

        public static float[,] AddVector(this float[,] _matrix1, float[,] _matrix2)
        {
            var matrix1Rows = _matrix1.GetLength(0);
            var matrix1Cols = _matrix1.GetLength(1);
            var matrix2Rows = _matrix2.GetLength(0);

            if (matrix1Rows != matrix2Rows)
                throw new InvalidOperationException
                  ("Product is undefined. n columns of first matrix must equal to n rows of second matrix");

            float[,] product = new float[matrix1Rows, matrix1Cols];

            for (int x = 0; x < matrix1Rows; x++)
            {
                for (int y = 0; y < matrix1Cols; y++)
                {
                    product[x, y] = _matrix1[x, y] + _matrix2[x, 0];
                }
            }

            return product;
        }

        public static float[,] Add(this float[,] _matrix, float[,] _by)
        {
            int _matrixWidth = _matrix.GetLength(0);
            int _matrixHeight = _matrix.GetLength(1);

            int _byWidth = _by.GetLength(0);
            int _byHeight = _by.GetLength(1);

            if (_matrixWidth != _byWidth ||
                _matrixHeight != _byHeight)
                throw new ArgumentException("Matrix multiplication not possible");

            float[,] c = new float[_matrixWidth, _matrixHeight];
            for (int x = 0; x < _matrixWidth; x++)
            {
                for (int y = 0; y < _matrixHeight; y++)
                {
                    c[x, y] = _matrix[x, y] + _by[x, y];
                }
            }

            return c;
        }

        public static float[,] Subtract(this float[,] _matrix, float[,] _by)
        {
            int _matrixWidth = _matrix.GetLength(0);
            int _matrixHeight = _matrix.GetLength(1);

            int _byWidth = _by.GetLength(0);
            int _byHeight = _by.GetLength(1);

            if (_matrixWidth != _byWidth ||
                _matrixHeight != _byHeight)
                throw new ArgumentException("Matrix multiplication not possible");

            float[,] c = new float[_matrixWidth, _matrixHeight];
            for (int x = 0; x < _matrixWidth; x++)
            {
                for (int y = 0; y < _matrixHeight; y++)
                {
                    c[x, y] = _matrix[x, y] - _by[x, y];
                }
            }

            return c;
        }

        public static float[,] Multiply(this float[,] _matrix, float _scalar)
        {
            int _matrixWidth = _matrix.GetLength(0);
            int _matrixHeight = _matrix.GetLength(1);

            float[,] c = new float[_matrixWidth, _matrixHeight];
            for (int x = 0; x < _matrixWidth; x++)
            {
                for (int y = 0; y < _matrixHeight; y++)
                {
                    c[x, y] = _matrix[x, y] * _scalar;
                }
            }

            return c;
        }

        public static float[,] Multiply(this float[,] _matrix, float[,] _other)
        {
            int _matrixWidth = _matrix.GetLength(0);
            int _matrixHeight = _matrix.GetLength(1);

            int _byWidth = _other.GetLength(0);
            int _byHeight = _other.GetLength(1);

            if (_matrixWidth != _byWidth ||
                _matrixHeight != _byHeight)
                throw new ArgumentException("Matrix multiplication not possible");

            float[,] c = new float[_matrixWidth, _byHeight];
            for (int x = 0; x < _matrixWidth; x++)
            {
                for (int y = 0; y < _matrixHeight; y++)
                {
                    c[x, y] = _matrix[x, y] * _other[x, y];
                }
            }

            return c;
        }
        #endregion -----------------------------------------------------------------

        public static T[,] Transpose<T>(this T[,] matrix)
        {
            int w = matrix.GetLength(0);
            int h = matrix.GetLength(1);

            T[,] result = new T[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = matrix[i, j];
                }
            }

            return result;
        }


        public static T[,] ReverseY<T>(this T[,] _colorArray)
        {
            T[,] newArray = new T[_colorArray.GetLength(0), _colorArray.GetLength(1)];
            for (int x = 0; x < _colorArray.GetLength(0); x++)
            {
                for (int yIn = 0, yOut = _colorArray.GetLength(1) - 1; yIn < _colorArray.GetLength(1); yIn++, yOut--)
                {
                    newArray[x, yOut] = _colorArray[x, yIn];
                }
            }

            return newArray;
        }

        public static float[,] SoftMax(float[,] _matrix)
        {
            float[,] softmax = new float[_matrix.GetLength(0), _matrix.GetLength(1)];

            for (int y = 0; y < _matrix.GetLength(1); y++)
            {
                float expForBatch = (float)Enumerable.Range(0, _matrix.GetLength(0)).Select(i => Math.Exp(_matrix[i, y])).Sum();

                for (int x = 0; x < _matrix.GetLength(0); x++)
                {
                    softmax[x, y] = (float)Math.Exp(_matrix[x, y]) / expForBatch;
                }
            }

            return softmax;
        }

        public static ArrayDimensionConvertData<T> To1DArray<T>(this T[,] _matrix)
        {
            int width = _matrix.GetLength(0);
            int height = _matrix.GetLength(1);
            T[] array = new T[width * height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    array[x + y * width] = _matrix[x, y];
                }
            }

            return new ArrayDimensionConvertData<T>(array, width);
        }

        public static T[,] To2DArray<T>(this ArrayDimensionConvertData<T> _data)
        {
            int width = _data.MatrixWidth;
            int height = _data.Array.Length / _data.MatrixWidth;

            T[,] matrix = new T[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    matrix[x, y] = _data.Array[x + y * width];
                }
            }

            return matrix;
        }

        public static string ToCsvString<T>(this T[,] _matrix)
        {
            string inputString = "";

            for (int x = 0; x < _matrix.GetLength(0); x++)
            {
                for (int y = 0; y < _matrix.GetLength(1); y++)
                {
                    inputString += _matrix[x, y].ToString();
                    if (y != _matrix.GetLength(1) - 1)
                        inputString += ",";
                }

                if (x != _matrix.GetLength(0) - 1)
                    inputString += "\n";
            }

            return inputString;
        }
    }
}
