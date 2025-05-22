using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace ImageDuplicateFinder {
    public static class Resize {
        public static void ResizeImages(string[] images, string path, int width) {
            Directory.CreateDirectory(path);

            ParallelHelper.For(images, 50, i => {
                var image = images[i];

                if (!ImageHelper.ImageExtensions.Contains(Path.GetExtension(image))) return;

                using (FileStream file = new FileStream($"{path}\\{Path.GetFileName(image)}", FileMode.OpenOrCreate))
                using (BinaryWriter writer = new BinaryWriter(file)) {
                    try {
                        Image img = Image.FromFile(image);
                        LockedBitmap bitmap = new LockedBitmap(new Bitmap(img, width, (int)(width / (float)img.Width * img.Height)), image);

                        writer.Write(img.Width);
                        writer.Write(img.Height);

                        byte[] byteArray = ZLib.Compress(bitmap.RGBValues).ToArray();
                        writer.Write(byteArray.Length);

                        writer.Write(byteArray);

                        bitmap.Dispose();
                        img.Dispose();
                    } catch {
                        file.Dispose();
                        File.Delete($"{path}\\{Path.GetFileName(image)}");
                    }
                }
            });
        }
    }
}
