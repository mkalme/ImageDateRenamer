using System;
using System.Collections.Generic;
using System.Drawing;

namespace ImageDuplicateFinder {
    public class ImageController : IImageController, IDisposable {
        private Dictionary<string, LockedBitmap> _bitmaps = new Dictionary<string, LockedBitmap>();
        private bool disposedValue;

        public int Limit { get; set; } = 10;

        public LockedBitmap GetBitmap(string image) {
            LockedBitmap bitmap;
            if (_bitmaps.TryGetValue(image, out bitmap)) return bitmap;

            bitmap = new LockedBitmap(new Bitmap(image), image);

            if (_bitmaps.Count < Limit) {
                _bitmaps.Add(image, bitmap);
            }

            return bitmap;
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    foreach (var bitmap in _bitmaps) {
                        bitmap.Value.Dispose();
                    }

                    _bitmaps.Clear();
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
