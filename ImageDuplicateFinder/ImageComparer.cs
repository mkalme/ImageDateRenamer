using System;

namespace ImageDuplicateFinder {
    public class ImageComparer {
        public static bool Compare(LockedBitmap main, LockedBitmap other) {
            if (main.Width != other.Width || main.Height != other.Height) return false;
            if (main.Path == other.Path) return false;

            return main.RGBValues.Compare(other.RGBValues);
        }
    }
}
