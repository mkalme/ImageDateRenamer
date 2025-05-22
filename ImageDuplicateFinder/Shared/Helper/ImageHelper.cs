using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ImageDuplicateFinder {
    public static class ImageHelper {
        public static readonly HashSet<string> ImageExtensions = new HashSet<string>(new string[] { ".jpg", ".tif", ".bmp", ".png", ".jpeg" });

        public static bool[] CompareImages(LockedBitmap main, string[] images, IImageController controller, HashSet<string> imagesTaken) {
            bool[] duplicates = new bool[images.Length];

            Parallel.For(0, images.Length, i => {
                if (!ImageExtensions.Contains(Path.GetExtension(images[i])) || main.Path == images[i]) return;
                if (imagesTaken.Contains(images[i])) return;

                var otherImage = controller.GetBitmap(images[i]);
                duplicates[i] = ImageComparer.Compare(main, otherImage);
            });

            return duplicates;
        }
    }
}
