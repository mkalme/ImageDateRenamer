using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace ImageDuplicateFinder {
    public class LockedBitmap : IDisposable {
        private bool disposedValue;

        public byte[] RGBValues { get; set; }
        public string Path { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public LockedBitmap(Bitmap bitmap, string path) {
            Lock(bitmap);
            Path = path;
        }
        public LockedBitmap(Stream stream, string path) {
            using (BinaryReader reader = new BinaryReader(stream, System.Text.Encoding.UTF8, true)) {
                int width = reader.ReadInt32();
                int height = reader.ReadInt32();

                Path = path;
                RGBValues = reader.ReadBytes(reader.ReadInt32());

                Width = width;
                Height = height;
            }

            Path = path;
        }

        public void Lock(Bitmap bitmap) {
            Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData data = bitmap.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            int bytes = Math.Abs(data.Stride) * data.Height;
            byte[] rgbValues = new byte[bytes];

            Marshal.Copy(data.Scan0, rgbValues, 0, bytes);

            bitmap.UnlockBits(data);
            bitmap.Dispose();

            RGBValues = rgbValues;
            Width = data.Width;
            Height = data.Height;
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    RGBValues = null;
                }

                disposedValue = true;
            }
        }
        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}