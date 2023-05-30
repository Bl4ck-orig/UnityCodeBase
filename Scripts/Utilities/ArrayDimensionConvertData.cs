namespace Utilities
{
    public struct ArrayDimensionConvertData<T>
    {
        public T[] Array { get; set; }

        public int MatrixWidth { get; set; }

        public ArrayDimensionConvertData(T[] _array, int _matrixWidth)
        {
            Array = _array;
            MatrixWidth = _matrixWidth;
        }
    }
}
