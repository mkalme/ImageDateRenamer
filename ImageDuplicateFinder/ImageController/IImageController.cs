using System;

namespace ImageDuplicateFinder {
    public interface IImageController : IDisposable {
        LockedBitmap GetBitmap(string path);
    }
}
