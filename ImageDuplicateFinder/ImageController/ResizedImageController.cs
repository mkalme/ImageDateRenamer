using System;
using System.Collections.Concurrent;
using System.IO;

namespace ImageDuplicateFinder {
    public class ResizedImageController : IImageController {
        private ConcurrentDictionary<string, LockedBitmap> _bitmaps = new ConcurrentDictionary<string, LockedBitmap>();
        private bool disposedValue;

        public LockedBitmap GetBitmap(string path) {
            LockedBitmap output;

            if (!_bitmaps.TryGetValue(path, out output)) {
                using (FileStream stream = File.OpenRead(path)) {
                    output = new LockedBitmap(stream, path);

                    _bitmaps.TryAdd(path, output);
                }
            }

            return output;
        }

        protected virtual void Dispose(bool disposing) {
            if (!disposedValue) {
                if (disposing) {
                    foreach (var bitmap in _bitmaps) {
                        bitmap.Value.Dispose();
                    }
                }

                _bitmaps.Clear();

                disposedValue = true;
            }
        }

        public void Dispose() {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
