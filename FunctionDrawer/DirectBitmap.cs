using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace FunctionDrawer
{
    internal class DirectBitmap : IDisposable
    {
        private readonly int[] _bits;
        private readonly int _height;
        private readonly int _width;
        private bool _disposed;
        private GCHandle _bitsHandle;

        public Bitmap Bitmap => new Bitmap(_width, _height, _width * 4, PixelFormat.Format32bppPArgb,
            _bitsHandle.AddrOfPinnedObject());

        public int this[int i]
        {
            get { return _bits[i]; }
            set { _bits[i] = value; }
        }

        public DirectBitmap(int width, int height)
        {
            _width = width;
            _height = height;
            _bits = new int[width * height];
            _bitsHandle = GCHandle.Alloc(_bits, GCHandleType.Pinned);
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _bitsHandle.Free();
        }
    }
}